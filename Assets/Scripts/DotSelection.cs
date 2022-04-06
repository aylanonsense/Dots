using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dots
{
	public class DotSelection : MonoBehaviour
	{
		public List<Dot> dots { get; private set; } = new List<Dot>();
		public HashSet<Dot> uniqueDots { get; private set; } = new HashSet<Dot>();
		public List<LineSegment> lines { get; private set; } = new List<LineSegment>();
		public Dot currentDot => dots.Count > 0 ? dots[dots.Count - 1] : null;
		public Dot previousDot => dots.Count > 1 ? dots[dots.Count - 2] : null;
		public LineSegment currentLineSegment => lines.Count > 0 ? lines[lines.Count - 1] : null;
		public LineSegment previousLineSegment => lines.Count > 1 ? lines[lines.Count - 2] : null;
		public int numDots => dots.Count;
		public bool hasLoop => dots.Count > uniqueDots.Count;
		public int colorIndex
		{
			get => _colorIndex;
			set
			{
				_colorIndex = value;
				foreach (LineSegment line in lines)
					line.colorIndex = _colorIndex;
			}
		}
		public int minValidSelectionLength = 2;

		public event Action onFormLoop;
		public event Action onUnformLoop;

		[SerializeField] private PrefabPool lineSegmentPool;
		[SerializeField] private Vector3 lineOffset = new Vector3(0f, 0f, 1f);
		private int _colorIndex;

		public void Push(Dot dot)
		{
			bool didHaveLoop = hasLoop;
			// Pin the previous line to this newly-added dot
			if (lines.Count > 0)
				lines[lines.Count - 1].endPosition = dot.transform.position;
			// Add the dot
			dots.Add(dot);
			uniqueDots.Add(dot);
			// Create a new line segment
			LineSegment line = lineSegmentPool.Spawn<LineSegment>();
			line.transform.SetParent(transform);
			line.colorIndex = colorIndex;
			line.startPosition = dot.transform.position + lineOffset;
			line.endPosition = GetMousePosition() + lineOffset;
			lines.Add(line);
			if (!didHaveLoop && hasLoop)
				onFormLoop?.Invoke();
		}

		public void Pop()
		{
			if (dots.Count > 0)
			{
				bool didHaveLoop = hasLoop;
				dots.RemoveAt(dots.Count - 1);
				uniqueDots = new HashSet<Dot>(dots);
				// Despawn the last line segment
				LineSegment line = lines[lines.Count - 1];
				lines.RemoveAt(lines.Count - 1);
				line.Despawn();
				if (lines.Count > 0)
					lines[lines.Count - 1].endPosition = GetMousePosition() + lineOffset;
				// Move the new last line segment to the mouse
				if (didHaveLoop && !hasLoop)
					onUnformLoop?.Invoke();
			}
		}

		public bool Contains(Dot dot)
		{
			return uniqueDots.Contains(dot);
		}

		public bool ContainsLink(Dot dot1, Dot dot2)
		{
			for (int i = 1; i < dots.Count; i++)
			{
				if ((dots[i - 1] == dot1 && dots[i] == dot2) || (dots[i - 1] == dot2 && dots[i] == dot1))
					return true;
			}
			return false;
		}

		public void Clear()
		{
			bool didHaveLoop = hasLoop;
			foreach (LineSegment line in lines)
				line.Despawn();
			dots.Clear();
			uniqueDots.Clear();
			lines.Clear();
			if (didHaveLoop)
				onUnformLoop?.Invoke();
		}

		private void Update()
		{
			// Move the start and end positions of all lines to the positions of the dots
			//  (dots are mostly stationary so we don't ~necessarily~ need to do this every frame,
			//  but it is possible for the player to select lines that are in motion, so we do it)
			for (int i = 0; i < lines.Count; i++)
			{
				lines[i].startPosition = dots[i].transform.position + lineOffset;
				if (dots.Count > i + 1)
				{
					lines[i].endPosition = dots[i + 1].transform.position + lineOffset;
				}
				else
				{
					// The last line is pinned to the mouse
					lines[i].endPosition = GetMousePosition() + lineOffset;
				}
			}
		}

		private Vector3 GetMousePosition()
		{
			Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			return new Vector3(mousePosition.x, mousePosition.y, 0f);
		}
	}
}
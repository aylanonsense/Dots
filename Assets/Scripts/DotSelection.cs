using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dots
{
	public class DotSelection : MonoBehaviour
	{
		public List<Dot> dots { get; private set; } = new List<Dot>();
		public HashSet<Dot> uniqueDots { get; private set; } = new HashSet<Dot>();
		public Dot currentDot => dots.Count > 0 ? dots[dots.Count - 1] : null;
		public Dot previousDot => dots.Count > 1 ? dots[dots.Count - 2] : null;
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

		public event Action onHoverPreviousLineSegment;

		[SerializeField] private PrefabPool lineSegmentPool;
		[SerializeField] private float lineZOffset = 1f;
		private List<LineSegment> lines = new List<LineSegment>();
		private int _colorIndex;

		public void AddDot(Dot dot)
		{
			// Add the dot
			dots.Add(dot);
			uniqueDots.Add(dot);
			// Pin the last line segment's end position to that dot
			if (lines.Count > 0)
				lines[lines.Count - 1].endPosition = new Vector3(dot.transform.position.x, dot.transform.position.y, lineZOffset);
			// Create a new line segment with a start position pinned to that dot
			LineSegment line = lineSegmentPool.Spawn<LineSegment>();
			line.transform.SetParent(transform);
			line.colorIndex = colorIndex;
			line.startPosition = new Vector3(dot.transform.position.x, dot.transform.position.y, lineZOffset);
			if (lines.Count > 1)
				lines[lines.Count - 2].onHoverStart -= OnHoverPreviousLineSegment;
			lines.Add(line);
			if (lines.Count > 1)
				lines[lines.Count - 2].onHoverStart += OnHoverPreviousLineSegment;
			MoveLastLineSegmentToMouse();
		}

		public void RemoveMostRecentlyAddedDot()
		{
			if (dots.Count > 0)
			{
				dots.RemoveAt(dots.Count - 1);
				uniqueDots = new HashSet<Dot>(dots);
				// Despawn the last line segment
				LineSegment line = lines[lines.Count - 1];
				if (lines.Count > 1)
					lines[lines.Count - 2].onHoverStart -= OnHoverPreviousLineSegment;
				lines.RemoveAt(lines.Count - 1);
				line.Despawn();
				if (lines.Count > 1)
					lines[lines.Count - 2].onHoverStart += OnHoverPreviousLineSegment;
				// Move the new last line segment to the mouse
				MoveLastLineSegmentToMouse();
			}
		}

		public bool ContainsDot(Dot dot)
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
			dots.Clear();
			uniqueDots.Clear();
			if (lines.Count > 1)
				lines[lines.Count - 2].onHoverStart -= OnHoverPreviousLineSegment;
			foreach (LineSegment line in lines)
				line.Despawn();
			lines.Clear();
		}

		private void Update()
		{
			MoveLastLineSegmentToMouse();
		}

		private void MoveLastLineSegmentToMouse()
		{
			if (lines.Count > 0)
			{
				Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				lines[lines.Count - 1].endPosition = new Vector3(mousePosition.x, mousePosition.y, lineZOffset);
			}
		}

		private void OnHoverPreviousLineSegment()
		{
			onHoverPreviousLineSegment?.Invoke();
		}
	}
}
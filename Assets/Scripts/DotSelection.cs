using System.Collections.Generic;
using UnityEngine;

namespace Dots
{
	public class DotSelection : MonoBehaviour
	{
		[SerializeField] private PrefabPool lineSegmentPool;
		public Dot currentDot => dots.Count > 0 ? dots[dots.Count - 1] : null;
		public Dot previousDot => dots.Count > 1 ? dots[dots.Count - 2] : null;
		public int numDots => dots.Count;
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

		private List<Dot> dots = new List<Dot>();
		private List<LineSegment> lines = new List<LineSegment>();
		private int _colorIndex;

		public void AddDot(Dot dot)
		{
			// Add the dot
			dots.Add(dot);
			// Pin the last line segment's end position to that dot
			if (lines.Count > 0)
				lines[lines.Count - 1].endPosition = dot.transform.position;
			// Create a new line segment with a start position pinned to that dot
			LineSegment line = lineSegmentPool.Spawn<LineSegment>();
			line.transform.SetParent(transform);
			line.colorIndex = colorIndex;
			line.startPosition = dot.transform.position;
			lines.Add(line);
			MoveLastLineSegmentToMouse();
		}

		public void RemoveMostRecentlyAddedDot()
		{
			if (dots.Count > 0)
			{
				dots.RemoveAt(dots.Count - 1);
				// Despawn the last line segment
				LineSegment line = lines[lines.Count - 1];
				lines.RemoveAt(lines.Count - 1);
				line.Despawn();
				// Move the new last line segment to the mouse
				MoveLastLineSegmentToMouse();
			}
		}

		public bool ContainsDot(Dot dot)
		{
			return dots.Contains(dot);
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

		private void Update()
		{
			MoveLastLineSegmentToMouse();
		}

		private void MoveLastLineSegmentToMouse()
		{
			if (lines.Count > 0)
			{
				Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				lines[lines.Count - 1].endPosition = mousePosition;
			}
		}
	}
}
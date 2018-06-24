using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Myra.Attributes;
using Myra.Utility;

namespace Myra.Graphics2D.UI
{
	public class Grid : MultipleItemsContainer
	{
		public enum ProportionType
		{
			Auto,
			Part,
			Fill,
			Pixels
		}

		public class Proportion
		{
			private static readonly Proportion _default = new Proportion(ProportionType.Part, 1.0f);

			private ProportionType _type;
			private float _value = 1.0f;

			public static Proportion Default
			{
				get { return _default; }
			}

			[DefaultValue(ProportionType.Auto)]
			public ProportionType Type
			{
				get { return _type; }

				set
				{
					if (value == _type) return;
					_type = value;
					FireChanged();
				}
			}

			[DefaultValue(1.0f)]
			public float Value
			{
				get { return _value; }
				set
				{
					if (value.EpsilonEquals(_value))
					{
						return;
					}

					_value = value;
					FireChanged();
				}
			}

			public event EventHandler Changed;

			public Proportion()
			{
			}

			public Proportion(ProportionType type)
			{
				_type = type;
			}

			public Proportion(ProportionType type, float value)
				: this(type)
			{
				_value = value;
			}

			public override string ToString()
			{
				if (_type == ProportionType.Auto || _type == ProportionType.Fill)
				{
					return _type.ToString();
				}

				if (_type == ProportionType.Part)
				{
					return string.Format("{0}: {1:0.00}", _type, _value);
				}

				// Pixels
				return string.Format("{0}: {1}", _type, (int) _value);
			}

			private void FireChanged()
			{
				var ev = Changed;
				if (ev != null)
				{
					ev(this, EventArgs.Empty);
				}
			}
		}

		private int _columnSpacing;
		private int _rowSpacing;
		private readonly ObservableCollection<Proportion> _columnsProportions = new ObservableCollection<Proportion>();
		private readonly ObservableCollection<Proportion> _rowsProportions = new ObservableCollection<Proportion>();
		private float? _totalColumnsPart, _totalRowsPart;
		private readonly List<Rectangle> _gridLines = new List<Rectangle>();
		private readonly List<int> _cellLocationsX = new List<int>();
		private readonly List<int> _cellLocationsY = new List<int>();
		private readonly List<int> _gridLinesX = new List<int>();
		private readonly List<int> _gridLinesY = new List<int>();
		private Point _actualSize;

		private readonly List<int> _measureColWidths = new List<int>();
		private readonly List<int> _measureRowHeights = new List<int>();
		private readonly List<Widget> _visibleWidgets = new List<Widget>();
		private readonly List<int> _colWidths = new List<int>();
		private readonly List<int> _rowHeights = new List<int>();

		[EditCategory("Behavior")]
		[DefaultValue(false)]
		public bool DrawLines { get; set; }

		[EditCategory("Behavior")]
		public Color DrawLinesColor { get; set; }

		[EditCategory("Grid")]
		public virtual int ColumnSpacing
		{
			get { return _columnSpacing; }
			set
			{
				if (value == _columnSpacing)
				{
					return;
				}

				_columnSpacing = value;
				InvalidateMeasure();
			}
		}

		[EditCategory("Grid")]
		public virtual int RowSpacing
		{
			get { return _rowSpacing; }
			set
			{
				if (value == _rowSpacing)
				{
					return;
				}

				_rowSpacing = value;
				InvalidateMeasure();
			}
		}

		[EditCategory("Grid")]
		public virtual ObservableCollection<Proportion> ColumnsProportions
		{
			get { return _columnsProportions; }
		}

		[EditCategory("Grid")]
		public virtual ObservableCollection<Proportion> RowsProportions
		{
			get { return _rowsProportions; }
		}

		[EditCategory("Grid")]
		public virtual float? TotalRowsPart
		{
			get { return _totalRowsPart; }

			set
			{
				if (value == _totalRowsPart)
				{
					return;
				}

				_totalRowsPart = value;
				InvalidateLayout();
			}
		}

		[EditCategory("Grid")]
		public virtual float? TotalColumnsPart
		{
			get { return _totalColumnsPart; }

			set
			{
				if (value == _totalColumnsPart)
				{
					return;
				}

				_totalColumnsPart = value;
				InvalidateLayout();
			}

		}

		public Grid()
		{
			_columnsProportions.CollectionChanged += OnProportionsChanged;
			_rowsProportions.CollectionChanged += OnProportionsChanged;

			DrawLines = false;
			DrawLinesColor = Color.White;
		}

		public int GetColumnWidth(int index)
		{
			if (_colWidths == null || index < 0 || index >= _colWidths.Count)
			{
				return 0;
			}

			return _colWidths[index];
		}

		public int GetRowHeight(int index)
		{
			if (_rowHeights == null || index < 0 || index >= _rowHeights.Count)
			{
				return 0;
			}

			return _rowHeights[index];
		}

		public int GetCellLocationX(int col)
		{
			if (col < 0 || col >= _cellLocationsX.Count)
			{
				return 0;
			}

			return _cellLocationsX[col];
		}

		public int GetCellLocationY(int row)
		{
			if (row < 0 || row >= _cellLocationsY.Count)
			{
				return 0;
			}

			return _cellLocationsY[row];
		}

		public Rectangle GetCellRectangle(int col, int row)
		{
			if (col < 0 || col >= _cellLocationsX.Count ||
			    row < 0 || row >= _cellLocationsY.Count)
			{
				return Rectangle.Empty;
			}

			return new Rectangle(_cellLocationsX[col], _cellLocationsY[row],
				_colWidths[col], _rowHeights[row]);
		}

		private void OnProportionsChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			if (args.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (var i in args.NewItems)
				{
					((Proportion) i).Changed += OnProportionsChanged;
				}
			}
			else if (args.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (var i in args.OldItems)
				{
					((Proportion) i).Changed -= OnProportionsChanged;
				}
			}

			InvalidateMeasure();
		}

		private void OnProportionsChanged(object sender, EventArgs args)
		{
			InvalidateMeasure();
		}

		public Proportion GetColumnProportion(int col)
		{
			if (col < 0 || col >= ColumnsProportions.Count)
			{
				return Proportion.Default;
			}

			return ColumnsProportions[col];
		}

		public Proportion GetRowProportion(int row)
		{
			if (row < 0 || row >= RowsProportions.Count)
			{
				return Proportion.Default;
			}

			return RowsProportions[row];
		}

		private Point GetActualGridPosition(Widget child)
		{
			var result = new Point(child.GridPositionX, child.GridPositionY);

			if (result.X > ColumnsProportions.Count)
			{
				result.X = ColumnsProportions.Count;
			}

			if (result.Y > RowsProportions.Count)
			{
				result.Y = RowsProportions.Count;
			}

			return result;
		}

		private Point LayoutProcessFixed(Point availableSize)
		{
			var rows = 0;
			var columns = 0;

			_visibleWidgets.Clear();
			foreach (var child in Widgets)
			{
				if (child.Visible)
				{
					_visibleWidgets.Add(child);

					var gridPosition = GetActualGridPosition(child);
					if (gridPosition.X + child.GridSpanX > columns)
					{
						columns = gridPosition.X + child.GridSpanX;
					}

					if (gridPosition.Y + child.GridSpanY > rows)
					{
						rows = gridPosition.Y + child.GridSpanY;
					}
				}
			}

			if (ColumnsProportions.Count > columns)
			{
				columns = ColumnsProportions.Count;
			}

			if (RowsProportions.Count > rows)
			{
				rows = RowsProportions.Count;
			}

			_measureColWidths.Clear();
			int i;
			for (i = 0; i < columns; ++i)
			{
				_measureColWidths.Add(0);
			}

			_measureRowHeights.Clear();
			for (i = 0; i < rows; ++i)
			{
				_measureRowHeights.Add(0);
			}

			availableSize.X -= (_measureColWidths.Count - 1)*_columnSpacing;
			availableSize.Y -= (_measureRowHeights.Count - 1)*_rowSpacing;

			for (var row = 0; row < rows; ++row)
			{
				for (var col = 0; col < columns; ++col)
				{
					var rowProportion = GetRowProportion(row);
					var colProportion = GetColumnProportion(col);

					if (colProportion.Type == ProportionType.Pixels)
					{
						_measureColWidths[col] = (int) colProportion.Value;
					}

					if (rowProportion.Type == ProportionType.Pixels)
					{
						_measureRowHeights[row] = (int) rowProportion.Value;
					}

					foreach (var widget in _visibleWidgets)
					{
						var gridPosition = GetActualGridPosition(widget);
						if (gridPosition.X != col ||
						    gridPosition.Y != row)
						{
							continue;
						}

						var measuredSize = Point.Zero;
						if (rowProportion.Type != ProportionType.Pixels ||
						    colProportion.Type != ProportionType.Pixels)
						{
							measuredSize = widget.Measure(availableSize);
						}

						if (widget.GridSpanX != 1)
						{
							measuredSize.X = 0;
						}

						if (widget.GridSpanY != 1)
						{
							measuredSize.Y = 0;
						}

						if (measuredSize.X > _measureColWidths[col])
						{
							_measureColWidths[col] = measuredSize.X;
						}

						if (measuredSize.Y > _measureRowHeights[row])
						{
							_measureRowHeights[row] = measuredSize.Y;
						}
					}
				}
			}

			var result = Point.Zero;

			for (i = 0; i < _measureColWidths.Count; ++i)
			{
				var w = _measureColWidths[i];

/*				if (result.X + w > availableSize.X)
				{
					colsWidths[i] = availableSize.X - result.X;
					result.X += colsWidths[i];
					break;
				}*/

				result.X += w;
				if (i < _measureColWidths.Count - 1)
				{
					result.X += _columnSpacing;
				}
			}

			for (i = 0; i < _measureRowHeights.Count; ++i)
			{
				var h = _measureRowHeights[i];
				result.Y += h;

				if (i < _measureRowHeights.Count - 1)
				{
					result.Y += _rowSpacing;
				}
			}

			return result;
		}

		public override void Arrange()
		{
			var bounds = ActualBounds;
			LayoutProcessFixed(bounds.Size());

			_colWidths.Clear();
			for (var i = 0; i < _measureColWidths.Count; ++i)
			{
				_colWidths.Add(_measureColWidths[i]);
			}

			_rowHeights.Clear();
			for (var i = 0; i < _measureRowHeights.Count; ++i)
			{
				_rowHeights.Add(_measureRowHeights[i]);
			}

			// Partition available space
			int row, col;

			// Dynamic widths
			// First run: calculate available width
			var availableWidth = (float) bounds.Width;
			availableWidth -= (_colWidths.Count - 1)*_columnSpacing;

			var totalPart = 0.0f;
			for (col = 0; col < _colWidths.Count; ++col)
			{
				var colWidth = _colWidths[col];
				var prop = GetColumnProportion(col);
				if (prop.Type == ProportionType.Auto || prop.Type == ProportionType.Pixels)
				{
					// Fixed width
					availableWidth -= colWidth;
				}
				else
				{
					totalPart += prop.Value;
				}
			}

			if (TotalColumnsPart.HasValue)
			{
				totalPart = TotalColumnsPart.Value;
			}

			if (!totalPart.IsZero())
			{
				// Second run update dynamic widths
				var tookSpace = 0.0f;
				for (col = 0; col < _colWidths.Count; ++col)
				{
					var prop = GetColumnProportion(col);
					if (prop.Type == ProportionType.Part)
					{
						_colWidths[col] = (int) (prop.Value*availableWidth/totalPart);
						tookSpace += _colWidths[col];
					}
				}

				availableWidth -= tookSpace;
			}

			// Update part fill widths
			for (col = 0; col < _colWidths.Count; ++col)
			{
				var prop = GetColumnProportion(col);
				if (prop.Type == ProportionType.Fill)
				{
					_colWidths[col] = (int) availableWidth;
					break;
				}
			}

			// Same with row heights
			var availableHeight = (float) bounds.Height;
			availableHeight -= (_rowHeights.Count - 1)*_rowSpacing;

			totalPart = 0.0f;
			for (col = 0; col < _rowHeights.Count; ++col)
			{
				var colHeight = _rowHeights[col];
				var prop = GetRowProportion(col);
				if (prop.Type == ProportionType.Auto || prop.Type == ProportionType.Pixels)
				{
					// Fixed height
					availableHeight -= colHeight;
				}
				else
				{
					totalPart += prop.Value;
				}
			}

			if (TotalRowsPart.HasValue)
			{
				totalPart = TotalRowsPart.Value;
			}

			if (!totalPart.IsZero())
			{
				var tookSpace = 0.0f;
				for (row = 0; row < _rowHeights.Count; ++row)
				{
					var prop = GetRowProportion(row);
					if (prop.Type != ProportionType.Part) continue;

					_rowHeights[row] = (int) (prop.Value*availableHeight/totalPart);
					tookSpace += _rowHeights[row];
				}

				availableHeight -= tookSpace;
			}

			// Update part fill heights
			for (row = 0; row < _rowHeights.Count; ++row)
			{
				var prop = GetRowProportion(row);
				if (prop.Type == ProportionType.Fill)
				{
					_rowHeights[row] = (int) availableHeight;
					break;
				}
			}

			_actualSize = Point.Zero;
			_gridLinesX.Clear();
			_cellLocationsX.Clear();
			var p = bounds.Location;
			for (var i = 0; i < _colWidths.Count; ++i)
			{
				_cellLocationsX.Add(p.X);
				var w = _colWidths[i];
				p.X += w;

				if (i < _colWidths.Count - 1)
				{
					_gridLinesX.Add(p.X + _columnSpacing/2);
				}

				p.X += _columnSpacing;

				_actualSize.X += _colWidths[i];
			}

			_gridLinesY.Clear();
			_cellLocationsY.Clear();
			for (var i = 0; i < _rowHeights.Count; ++i)
			{
				_cellLocationsY.Add(p.Y);
				var h = _rowHeights[i];
				p.Y += h;

				if (i < _rowHeights.Count - 1)
				{
					_gridLinesY.Add(p.Y + _rowSpacing/2);
				}

				p.Y += _rowSpacing;

				_actualSize.Y += _rowHeights[i];
			}

			_gridLines.Clear();
			foreach (var control in _visibleWidgets)
			{
				LayoutControl(control);
			}
		}

		private void LayoutControl(Widget control)
		{
			var gridPosition = GetActualGridPosition(control);
			var col = gridPosition.X;
			var row = gridPosition.Y;

			var cellSize = Point.Zero;

			for (var i = col; i < col + control.GridSpanX; ++i)
			{
				cellSize.X += _colWidths[i];
			}

			for (var i = row; i < row + control.GridSpanY; ++i)
			{
				cellSize.Y += _rowHeights[i];
			}

			control.Layout(new Rectangle(_cellLocationsX[col], _cellLocationsY[row], cellSize.X, cellSize.Y));
		}

		public override void InternalRender(RenderContext context)
		{
			base.InternalRender(context);

			if (!DrawLines)
			{
				return;
			}

			int i;
			var bounds = ActualBounds;
			for (i = 0; i < _gridLinesX.Count; ++i)
			{
				var x = _gridLinesX[i];
				context.Batch.DrawLine(x, bounds.Top, x, bounds.Bottom, DrawLinesColor);
			}

			for (i = 0; i < _gridLinesY.Count; ++i)
			{
				var y = _gridLinesY[i];
				context.Batch.DrawLine(bounds.Left, y, bounds.Right, y, DrawLinesColor);
			}
		}

		protected override Point InternalMeasure(Point availableSize)
		{
			return LayoutProcessFixed(availableSize);
		}
	}
}
using Microsoft.Xna.Framework.Input;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public class VerticalMenu : Menu
	{
		public override Orientation Orientation
		{
			get { return Orientation.Vertical; }
		}

		[HiddenInEditor]
		[JsonIgnore]
		public override int? SelectedIndex
		{
			get
			{
				int index = 0;
				foreach (var item in Items)
				{
					var asMenuItem = item as MenuItem;
					if (asMenuItem != null && asMenuItem.Enabled)
					{
						if (asMenuItem.Widget.IsMouseOver)
						{
							return index;
						}

						++index;
					}
				}

				return null;
			}

			set
			{
				if (value == null)
				{
					Click(null);
					return;
				}

				if (ActiveItemsCount == 0)
				{
					return;
				}

				if (value.Value < 0)
				{
					value = ActiveItemsCount - 1;
				}

				if (value.Value >= ActiveItemsCount)
				{
					value = 0;
				}

				int index = 0;
				foreach (var item in Items)
				{
					if (index == value)
					{
						Click(item);
						break;
					}

					if (item.Widget is MenuItemButton && item.Widget.Enabled)
					{
						++index;
					}
				}
			}
		}

		public VerticalMenu(MenuStyle style) : base(style)
		{
			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Stretch;
		}

		public VerticalMenu(string style)
			: this(Stylesheet.Current.VerticalMenuVariants[style])
		{
		}

		public VerticalMenu()
			: base(Stylesheet.Current.VerticalMenuStyle)
		{
		}

		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			switch (k)
			{
				case Keys.Up:
					break;
				case Keys.Down:
					break;
			}
		}
	}
}

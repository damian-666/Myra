using Microsoft.Xna.Framework.Input;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public class HorizontalMenu : Menu
	{
		public override Orientation Orientation
		{
			get { return Orientation.Horizontal; }
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
					if (item.Widget == OpenMenuItem)
					{
						return index;
					}

					if (item.Widget is MenuItemButton && item.Widget.Enabled)
					{
						++index;
					}
				}

				return null;
			}

			set
			{
				if (value == null)
				{
					Select(null);
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
						Select(item);
						break;
					}

					if (item.Widget is MenuItemButton && item.Widget.Enabled)
					{
						++index;
					}
				}
			}
		}

		public HorizontalMenu(MenuStyle style) : base(style)
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Top;
		}

		public HorizontalMenu(string style) : this(Stylesheet.Current.HorizontalMenuVariants[style])
		{
		}

		public HorizontalMenu() : base(Stylesheet.Current.HorizontalMenuStyle)
		{
		}

		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			switch (k)
			{
				case Keys.Left:
					--SelectedIndex;
					break;
				case Keys.Right:
					++SelectedIndex;
					break;
			}
		}
	}
}

/* Generated by Myra UI Editor at 11/16/2017 2:38:36 AM */

using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;

namespace Myra.Samples.TabControlSample
{
	public partial class AllWidgets: Grid
	{
		public AllWidgets()
		{
			BuildUI();

			_button.Image = DefaultAssets.UISpritesheet["icon-star"];
			_blueButton.Image = DefaultAssets.UISpritesheet["icon-star"];
			_imageButton.Image = DefaultAssets.UISpritesheet["icon-star-outline"];

			_button.Up += (sender, args) =>
			{
				var debugWindow = new DebugOptionsDialog();
				debugWindow.ShowModal(Desktop);
			};

			_blueButton.Up += (sender, args) =>
			{
				var debugWindow = new DebugOptionsDialog();
				debugWindow.ShowModal(Desktop);
			};

			_textButton.Up += (sender, args) =>
			{
				var debugWindow = new DebugOptionsDialog();
				debugWindow.ShowModal(Desktop);
			};

			_imageButton.Up += (sender, args) =>
			{
				var debugWindow = new DebugOptionsDialog();
				debugWindow.ShowModal(Desktop);
			};

			_menuItemAbout.Selected += (sender, args) =>
			{
				var messageBox = Dialog.CreateMessageBox("AllWidgets", "Myra AllWidgets Sample " + MyraEnvironment.Version);
				messageBox.ShowModal(Desktop);
			};

			var tree = new Tree
			{
				HasRoot = false,
				GridPositionX = 1,
				GridPositionY = 11
			};
			var node1 = tree.AddSubNode("node1");
			var node2 = node1.AddSubNode("node2");
			var node3 = node2.AddSubNode("node3");
			node3.AddSubNode("node4");
			node3.AddSubNode("node5");
			node2.AddSubNode("node6");
			_gridRight.Widgets.Add(tree);
		}

		public override void InternalRender(RenderContext context)
		{
			_horizontalProgressBar.Value += 0.5f;
			if (_horizontalProgressBar.Value > _horizontalProgressBar.Maximum)
			{
				_horizontalProgressBar.Value = _horizontalProgressBar.Minimum;
			}

			_verticalProgressBar.Value += 0.5f;
			if (_verticalProgressBar.Value > _verticalProgressBar.Maximum)
			{
				_verticalProgressBar.Value = _verticalProgressBar.Minimum;
			}

			base.InternalRender(context);
		}
	}
}
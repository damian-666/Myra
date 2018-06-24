using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Myra.Graphics2D.Text
{
	public class GlyphRun
	{
		private string _text;
		private readonly SpriteFont _spriteFont;
		private bool _dirty = true;
		private Point _size;

		public int Count
		{
			get
			{
				return _text != null ? _text.Length : 0; 
			}
		}

		public string Text
		{
			get
			{
				return _text;
			}
		}

		public Point Size
		{
			get
			{
				Update();

				return _size;
			}
		}

		public SpriteFont SpriteFont
		{
			get { return _spriteFont; }
		}

		public int? UnderscoreIndex { get; set; }

		public GlyphRun(SpriteFont font)
		{
			if (font == null)
			{
				throw new ArgumentNullException("font");
			}

			_spriteFont = font;
		}

		public void Clear()
		{
			_size = Point.Zero;
			_text = string.Empty;
		}

		public void Append(CharInfo ci, Color? color)
		{
			_dirty = true;
			_text += ci.Value;
		}

		public void Append(IEnumerable<CharInfo> charInfos, Color? color)
		{
			foreach (var ci in charInfos)
			{
				Append(ci, color);
			}
		}

		private void Update()
		{
			if (!_dirty)
			{
				return;
			}

			if (!string.IsNullOrEmpty(_text))
			{
				var sz = _spriteFont.MeasureString(_text);
				_size = new Point((int)sz.X, (int)sz.Y);
			}
			else
			{
				_size = new Point(0, _spriteFont.LineSpacing);
			}

			_dirty = false;
		}

		public void Draw(SpriteBatch batch, Point pos, Color color)
		{
			Update();

			batch.DrawString(_spriteFont, _text, pos.ToVector2(), color);
		}
	}
}
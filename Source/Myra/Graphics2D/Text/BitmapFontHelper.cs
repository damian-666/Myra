﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cyotek.Drawing.BitmapFont;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;

namespace Myra.Graphics2D.Text
{
	public static class SpriteFontHelper
	{
		/// <summary>
		/// Ensures that the specified text contains valid description for BMFont
		/// </summary>
		/// <param name="input"></param>
		public static void Validate(string input)
		{
			var data = new BitmapFont();
			data.LoadText(input);
		}

		public static SpriteFont LoadFromFnt(string text, TextureRegion textureRegion)
		{
			var data = new BitmapFont();
			data.LoadText(text);

			var glyphBounds = new List<Rectangle>();
			var cropping = new List<Rectangle>();
			var chars = new List<char>();
			var kerning = new List<Vector3>();

			foreach (var pair in data.Characters)
			{
				var character = pair.Value;

				var bounds = character.Bounds;

				bounds.Offset(textureRegion.Bounds.Location);

				glyphBounds.Add(bounds);
				cropping.Add(new Rectangle(0, character.Offset.Y, bounds.Width, bounds.Height));

				chars.Add(pair.Key);

				kerning.Add(new Vector3(character.Offset.X, character.Bounds.Width, character.XAdvance - character.Bounds.Width));
			}

			var constructorInfo = typeof(SpriteFont).GetTypeInfo().DeclaredConstructors.First();
			var result = (SpriteFont) constructorInfo.Invoke(new object[]
			{
				textureRegion.Texture, glyphBounds, cropping,
				chars, data.LineHeight, 0, kerning, ' '
			});

			return result;
		}
	}
}
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Myra.Graphics2D;
using Myra.Graphics2D.TextureAtlases;

namespace Myra.Content.Pipeline.TextureAtlas
{
	[ContentImporter(".atlas", DefaultProcessor = "TextureAtlasProcessor",
		DisplayName = "LibGDX Texture Atlas Importer - Myra")]	
	public class TextureAtlasImporter: ContentImporter<TextureAtlasContent>
	{
		public override TextureAtlasContent Import(string filename, ContentImporterContext context)
		{
			string data;
			using (var streamReader = new StreamReader(filename))
			{
				data = streamReader.ReadToEnd();
			}

			return TextureAtlasContentLoader.Load(data);
		}
	}
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;


namespace Sweeper.GameObjects
{
    public abstract class TileModifier
    {
        public abstract void Draw(Rectangle tileRect, MapTile tile, SpriteBatch spriteBatch, Dictionary<string, Texture2D> textues, SpriteFont font);

        public virtual bool Detectable => false;

        public virtual bool CanEnter => true;

        public virtual void Enter(MapTile tile)
        {
            tile.Discovered = true;
        }
    }

    public class Empty : TileModifier
    {
        public override void Draw(Rectangle tileRect, MapTile tile, SpriteBatch spriteBatch, Dictionary<string, Texture2D> textues, SpriteFont font)
        {
            var color = tile.Map.Scene.Player.Trail.Contains(tile) ? Color.LightGreen : Color.DarkGreen;
            var fg = tile.Map.Scene.Player.Trail.Contains(tile) ? Color.Black : Color.White;
            var texture = textues["GridCell"];
            var encText = tile.Map.GetEncryption(tile.Location);
            spriteBatch.Draw(texture, tileRect, color);
            if (tile.DiscoveredNodes > 0)
                spriteBatch.DrawString(font, encText.Convert(tile.DiscoveredNodes).ToString(), new Vector2(tile.Location.X * 48 + 8, tile.Location.Y * 48 + 8), fg);
        }

        public override void Enter(MapTile tile)
        {
            tile.Map.Scene.ResolveTile(tile);
            base.Enter(tile);
        }
    }

    public class Blocked : TileModifier
    {
        public override void Draw(Rectangle tileRect, MapTile tile, SpriteBatch spriteBatch, Dictionary<string, Texture2D> textues, SpriteFont font)
        {
            var texture = textues["GridCell"];
            spriteBatch.Draw(texture, tileRect, Color.Black);
        }

        public override bool CanEnter => false;
    }

    public class Node : TileModifier
    {
        public override void Draw(Rectangle tileRect, MapTile tile, SpriteBatch spriteBatch, Dictionary<string, Texture2D> textures, SpriteFont font)
        {
            var texture = textures["GridCell"];
            spriteBatch.Draw(texture, tileRect, Color.Red);            
            spriteBatch.Draw(textures["HackedNode"], tileRect, Color.White);
        }

        public override bool Detectable => true;

        public override void Enter(MapTile tile)
        {
            if (!tile.Discovered)
            {
                tile.Map.Scene.FloatText("Unauthorized access attempt!", tile, Color.OrangeRed);
                tile.Map.Scene.Penalties.Add(TracePenalty.Alert);
            }
            base.Enter(tile);
        }
    }

    public class HackedNode : TileModifier
    {
        public override void Draw(Rectangle tileRect, MapTile tile, SpriteBatch spriteBatch, Dictionary<string, Texture2D> textures, SpriteFont font)
        {
            var texture = textures["GridCell"];
            spriteBatch.Draw(texture, tileRect, Color.DarkGreen);
            spriteBatch.Draw(textures["Node"], tileRect, Color.White);
        }

        public override bool Detectable => true;
    }

    public class Encrypted : Empty
    {
        public override void Draw(Rectangle tileRect, MapTile tile, SpriteBatch spriteBatch, Dictionary<string, Texture2D> textures, SpriteFont font)
        {
            base.Draw(tileRect, tile, spriteBatch, textures, font);
            var texture = textures["GridCell"];
            var pickupRect = new Rectangle(tileRect.Location.Offset(6, 6), tileRect.Size.Offset(-12, -12));
            spriteBatch.Draw(texture, pickupRect, Color.OrangeRed);
            spriteBatch.DrawString(font, "?", tileRect
                .Location.Offset(8, 8).ToVector2(), Color.White);
        }

        public override void Enter(MapTile tile)
        {
            tile.Modifier = new Empty();
            tile.Map.Scene.FloatText("Encryption Broken", tile, Color.OrangeRed);
            tile.Map.Scene.Penalties.Add(TracePenalty.EncryptionBreak);
            base.Enter(tile);
        }
    }

    public class Corrupted : TileModifier
    {
        public override void Draw(Rectangle tileRect, MapTile tile, SpriteBatch spriteBatch, Dictionary<string, Texture2D> textures, SpriteFont font)
        {            
            var texture = textures["GridCell"];
            spriteBatch.Draw(texture, tileRect, Color.PaleVioletRed);
            spriteBatch.Draw(textures["Corrupt"], tileRect, Color.White);
        }
    }

    public class BitCoin : Empty
    {
        public override void Draw(Rectangle tileRect, MapTile tile, SpriteBatch spriteBatch, Dictionary<string, Texture2D> textures, SpriteFont font)
        {
            base.Draw(tileRect, tile, spriteBatch, textures, font);
            var texture = textures["Cypher"];            
            spriteBatch.Draw(texture, tileRect, Color.White);            
        }

        public override void Enter(MapTile tile)
        {
            tile.Modifier = new Empty();
            tile.Map.Scene.BitCoin++;
            tile.Map.Scene.FloatText("BitCoin collected", tile, Color.Yellow);
            base.Enter(tile);
        }
    }

    public class Decryptor : Empty
    {
        public override void Draw(Rectangle tileRect, MapTile tile, SpriteBatch spriteBatch, Dictionary<string, Texture2D> textures, SpriteFont font)
        {
            base.Draw(tileRect, tile, spriteBatch, textures, font);
            var texture = textures["GridCell"];
            var pickupRect = new Rectangle(tileRect.Location.Offset(6, 6), tileRect.Size.Offset(-12, -12));
            spriteBatch.Draw(texture, pickupRect, Color.Red);
            spriteBatch.DrawString(font, "D", tileRect
                .Location.Offset(8, 8).ToVector2(), Color.White);
        }

        public override void Enter(MapTile tile)
        {
            if (tile.Map.Scene.BitCoin >= 10)
            {
                tile.Map.Scene.BitCoin -= 10;
                foreach (var target in tile.Map.Tiles.Where(t => t.Modifier is Encrypted))
                {
                    target.Modifier = new Empty();
                }
                tile.Modifier = new Empty();                
            }
            base.Enter(tile);
        }
    }

    public class Uplink : Empty
    {
        public override void Draw(Rectangle tileRect, MapTile tile, SpriteBatch spriteBatch, Dictionary<string, Texture2D> textures, SpriteFont font)
        {
            base.Draw(tileRect, tile, spriteBatch, textures, font);
            var texture = textures["GridCell"];
            var pickupRect = new Rectangle(tileRect.Location.Offset(6, 6), tileRect.Size.Offset(-12, -12));
            spriteBatch.Draw(texture, pickupRect, Color.Blue);
            spriteBatch.DrawString(font, "R", tileRect
                .Location.Offset(8, 8).ToVector2(), Color.White);
        }

        public override void Enter(MapTile tile)
        {
            if (tile.Map.Scene.BitCoin >= 5)
            {
                tile.Map.Scene.BitCoin -= 5;
                tile.Map.Scene.ResetUsed = true;
                tile.Map.Scene.Player.Trail.Clear();
                tile.Modifier = new Empty();                
            }
            base.Enter(tile);
        }
    }    
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var color = tile.Map.Scene.Player.Trail.Contains(tile) ? Color.Yellow : Color.White;
            var texture = textues["GridCell"];
            var encText = tile.Map.GetEncryption(tile.Location);
            spriteBatch.Draw(texture, tileRect, color);
            if (tile.DiscoveredNodes > 0)
                spriteBatch.DrawString(font, encText.Convert(tile.DiscoveredNodes).ToString(), new Vector2(tile.Location.X * 48 + 8, tile.Location.Y * 48 + 8), Color.Black);
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
            spriteBatch.Draw(texture, tileRect, Color.Gray);
        }

        public override bool CanEnter => false;
    }

    public class Node : TileModifier
    {
        public override void Draw(Rectangle tileRect, MapTile tile, SpriteBatch spriteBatch, Dictionary<string, Texture2D> textues, SpriteFont font)
        {
            var texture = textues["GridCell"];
            spriteBatch.Draw(texture, tileRect, Color.Red);
        }

        public override bool Detectable => true;

        public override void Enter(MapTile tile)
        {
            tile.Map.Scene.WriteConsoleMessage("Unauthorized access attempt!");
            tile.Map.Scene.Penalties.Add(TracePenalty.NodeFault);
            base.Enter(tile);
        }
    }

    public class HackedNode : TileModifier
    {
        public override void Draw(Rectangle tileRect, MapTile tile, SpriteBatch spriteBatch, Dictionary<string, Texture2D> textues, SpriteFont font)
        {
            var texture = textues["GridCell"];
            spriteBatch.Draw(texture, tileRect, Color.Green);
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
            tile.Map.Scene.WriteConsoleMessage("Encryption Broken");
            tile.Map.Scene.Penalties.Add(TracePenalty.EncryptionBreak);
            base.Enter(tile);
        }
    }

    public class BitCoin : Empty
    {
        public override void Draw(Rectangle tileRect, MapTile tile, SpriteBatch spriteBatch, Dictionary<string, Texture2D> textures, SpriteFont font)
        {
            base.Draw(tileRect, tile, spriteBatch, textures, font);
            var texture = textures["GridCell"];
            var pickupRect = new Rectangle(tileRect.Location.Offset(6, 6), tileRect.Size.Offset(-12, -12));
            spriteBatch.Draw(texture, pickupRect, Color.Gold);
            spriteBatch.DrawString(font, "B", tileRect
                .Location.Offset(8, 8).ToVector2(), Color.Red);
        }

        public override void Enter(MapTile tile)
        {
            tile.Modifier = new Empty();
            MainScene.BitCoin++;
            tile.Map.Scene.WriteConsoleMessage("BitCoin collected");
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
            tile.Map.Scene.Player.Trail.Clear();
            tile.Modifier = new Empty();
            base.Enter(tile);
        }
    }    
}

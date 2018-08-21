using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
namespace SideScroller01
{
    class PickUpHealthPack : GameItem
    {

        private Vector2 origin;
        private int howMuch; // How much health to give player

        public PickUpHealthPack(Level inLevel, Vector2 startPos, int howMuchToGive)
        {

            this.InLevel = inLevel;
            this.Position = startPos;
            this.origin = new Vector2(Game1.SprHealthPack.Width / 2,
                Game1.SprHealthPack.Height / 2);
            this.GetLayerDepth(this.Position.Y);
            this.howMuch = howMuchToGive;

        }

        // Does not need to override
        public override void Draw(SpriteBatch SB)
        {
            SB.Draw(Game1.SprHealthPack, Camera.GetScreenPosition(this.Position),
                null, Color.White, 0f, origin, 1f, SpriteEffects.None, this.LayerDepth);
        }

        public override void GetPickedUp(Player p)
        {
            p.GetHealth(this.howMuch);
            base.GetPickedUp(p);
        }

        public override bool CheckCollision(Actor actor)
        {
            // 1) Are wer within Y Range
            if (actor.Position.Y > this.Position.Y - Actor.HIT_Y_RANGE
                && actor.Position.Y < this.Position.Y + Actor.HIT_Y_RANGE)
            {
                // 2) Are we touching
                float dist = Math.Abs(actor.Position.X - this.Position.X); // Distance from Player
                float minDist = this.HitArea + actor.HitArea; // Minimum distance for a collision

                if (dist < minDist) return true; // Collision detected

            }

            // No Collision
            return false;
        }
    }
}

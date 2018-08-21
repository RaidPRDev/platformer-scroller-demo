using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
namespace SideScroller01
{
    class PickUpStone : GameItem
    {

        private Vector2 origin;
        public PickUpStone(Level inLevel, Vector2 startPos)
        {

            this.InLevel = inLevel;
            this.Position = startPos;
            this.origin = new Vector2(Game1.SprRocks.Width / 2,
                Game1.SprRocks.Height / 2);
            this.GetLayerDepth(this.Position.Y);
        
        }

        // Does not need to override
        public override void Draw(SpriteBatch SB)
        {
            SB.Draw(Game1.SprRocks, Camera.GetScreenPosition(this.Position),
                null, Color.White, 0f, origin, 1f, SpriteEffects.None, this.LayerDepth);
        }

        public override void GetPickedUp(Player p)
        {
            p.GetStone();
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

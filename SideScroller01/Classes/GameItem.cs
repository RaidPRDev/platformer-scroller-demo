using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SideScroller01
{
    class GameItem
    {
        // Play area range 128pixels
        // Actor.HIT_Y_RANGE = 30 pixels
        // get 30 as a % of player area = 23.3475%
        // Convert this to a % of 0.2f <-- The player area possible LayerDepth value
        // 0.23475 * 0.2 = 0.0.046875f
        const float DEPTHCHECK = 0.046875f;  
        public Level InLevel;
        public Vector2 Position;
        public float LayerDepth;
        public int HitArea;

        public virtual void Update(GameTime gT)
        {

        }

        public virtual void Draw(SpriteBatch SB) 
        {

        }

        public virtual void TakeHit(DirectionTarget cameFrom)
        {

        }

        public virtual void GetPickedUp(Player p)
        {
            SoundManager.PlaySound("PickUpItem");
            this.InLevel.GameItems.Remove(this); // Remove Item

        }

        /// <summary>
        /// Used for Player to 'use' this gameItem, such as picking up gameItems or kicking trashCans
        /// </summary>
        public virtual bool CheckCollision(Actor actor)
        {
            return false;
        }

        /// <summary>
        /// Used inside of Trashcan (Hit State) and Knife classes to check for attack collision
        /// </summary>
        public bool CheckEnemyCollision(Actor actor)
        {
            // 1)  Is actor attackable
            if (actor.IsAttackable)
            {
                // 2) Are we within Y range
                if (actor.LayerDepth > this.LayerDepth - GameItem.DEPTHCHECK
                    && actor.LayerDepth < this.LayerDepth + GameItem.DEPTHCHECK)
                {
                    // 3) Are we colliding
                    float dist = Math.Abs(actor.Position.X - this.Position.X);
                    float minDist = this.HitArea + actor.HitArea;
                    if (dist < minDist)
                    {
                        // We are colliding
                        return true;
                    }
                }
            }
            // No collision
            return false;
        }

        public void SetPosition(Vector2 position)
        {
            this.Position = position;
            GetLayerDepth(this.Position.Y);
        }

        public void GetLayerDepth(float yPos)
        {
            // Get Actors position as a % of total play area
            int min = InLevel.PlayBounds.Top;
            int max = InLevel.PlayBounds.Bottom;
            int range = max - min;
            float percent = (yPos - (float)min) / (float)range;

            percent = 1f - percent;

            // Convert % to a value of Layer Depth Range
            // Player LayerDepth section 0.4(front) - 0.6(back) - Range 0.2f
            this.LayerDepth = percent * 0.2f + 0.4f + 0.001f;
        }



    }
}

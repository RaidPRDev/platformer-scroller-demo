using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SideScroller01
{
    class Rock : GameItem
    {
        const int BASE_SPEED = 15;
        const int BASE_DAMAGE = 30;
        public static Vector2 Offset = new Vector2(70, 90);

        private int speed;
        private Vector2 origin;
        private float zHeight;
        private Actor whoThrewMe;

        public Rock(Vector2 startPos, DirectionTarget thrownDir, Level inLevel, Actor whoThrewMe)
        {
            this.whoThrewMe = whoThrewMe;
            this.zHeight = startPos.Y;
            this.InLevel = inLevel;
            this.origin = new Vector2(Game1.SprRocks.Width / 2, Game1.SprRocks.Height / 2);
            GetLayerDepth(zHeight);
            if (thrownDir == DirectionTarget.Left)
            {
                this.speed = BASE_SPEED * -1;
                this.Position = new Vector2(
                    startPos.X - Offset.X, startPos.Y - Offset.Y);
            }
            else
            {
                this.speed = BASE_SPEED;
                this.Position = new Vector2(
                    startPos.X + Offset.X, startPos.Y - Offset.Y);
            }
        }

        public override void Update(GameTime gT)
        {
            this.Position.X += speed;

            // Check for Actor collisions
            for (int i = 0; i < InLevel.Actors.Count; i++)
            {
                if (this.whoThrewMe != InLevel.Actors[i])
                {
                    
                    if (this.CheckEnemyCollision(InLevel.Actors[i]))
                    {
                        // Which way we traveling
                        if (this.speed < 0) // Going Left
                        {
                            InLevel.Actors[i].GetKnockedDown(DirectionTarget.Left, BASE_DAMAGE);
                            // If I ever want a item to go through all actors and knock them down as well
                            // We dont use the following, as its for Removing the item after the first hit it finds.
                            InLevel.GameItems.Remove(this);
                            return;
                        }
                        else // Going Right
                        {
                            InLevel.Actors[i].GetKnockedDown(DirectionTarget.Right, BASE_DAMAGE);
                            // If I ever want a item to go through all actors and knock them down as well
                            // We dont use the following, as its for Removing the item after the first hit it finds.
                            InLevel.GameItems.Remove(this);
                            return;
                        }
                    }
                }
            }

            // Have we gotten off screem (PlayBounds)
            if (this.Position.X < InLevel.PlayBounds.Left 
                || this.Position.X > InLevel.PlayBounds.Right)
                InLevel.GameItems.Remove(this);

        }

        public override void Draw(SpriteBatch SB)
        {
            if (this.speed < 0) // I'm going left
                SB.Draw(Game1.SprRocks, Camera.GetScreenPosition(this.Position),
                    null, Color.White, 0f, this.origin, 1f, SpriteEffects.None, this.LayerDepth);
            else // I am going right
                SB.Draw(Game1.SprRocks, Camera.GetScreenPosition(this.Position),
                    null, Color.White, 0f, this.origin, 1f, SpriteEffects.None, this.LayerDepth);
        }
    }
}

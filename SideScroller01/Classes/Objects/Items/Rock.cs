using System;
using Microsoft.Xna.Framework;
using SideScroller01.Classes.Scene;

namespace SideScroller01.Classes.Objects.Items
{
    class Rock : GameItem
    {
        private const int BASE_SPEED = 15;
        private const int BASE_DAMAGE = 30;
        
        public Rock(Vector2 startPos, DirectionTarget thrownDir, Level inLevel, Actor throwingActor)
            : base(startPos, inLevel, throwingActor)
        {
            this.Sprite = TextureManager.GetTexure("Rocks");
            this.SetOffsetPosition(new Vector2(70, 90));
            this.InitSpriteFrames(this.Sprite.Width, this.Sprite.Height);
            this.SetOriginPosition();
            this.GetLayerDepth(startPos.Y);

            if (thrownDir == DirectionTarget.Left)
            {
                this.Speed.X = BASE_SPEED * -1;
                this.Position = new Vector2(startPos.X - Offset.X, startPos.Y - Offset.Y);
            }
            else
            {
                this.Speed.X = BASE_SPEED;
                this.Position = new Vector2(startPos.X + Offset.X, startPos.Y - Offset.Y);
            }
        }

        public override void Update(GameTime gT)
        {
            this.Position.X += this.Speed.X;

            // Find throwing actor and check for collisions
            for (int i = 0; i < InLevel.Actors.Count; i++)
            {
                if (this.FromActor != InLevel.Actors[i])
                {
                    if (this.CheckEnemyCollision(InLevel.Actors[i]))
                    {
                        // Which way we traveling
                        if (this.Speed.X < 0) // Going Left
                        {
                            this.InLevel.Actors[i].GetKnockedDown(DirectionTarget.Left, BASE_DAMAGE);
                            // If I ever want a item to go through all actors and knock them down as well
                            // We dont use the following, as its for Removing the item after the first hit it finds.
                            this.Destroy();
                            return;
                        }
                        else // Going Right
                        {
                            this.InLevel.Actors[i].GetKnockedDown(DirectionTarget.Right, BASE_DAMAGE);
                            // If I ever want a item to go through all actors and knock them down as well
                            // We dont use the following, as its for Removing the item after the first hit it finds.
                            this.Destroy();
                            return;
                        }
                    }
                }
            }

            // Have we gotten off screem (PlayBounds)
            if (this.Position.X < this.InLevel.PlayBounds.Left || this.Position.X > this.InLevel.PlayBounds.Right)
                this.Destroy();

        }
    }
}

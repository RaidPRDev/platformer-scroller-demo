using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SideScroller01.Classes.Objects.Items.Base;
using SideScroller01.Classes.Scene;

namespace SideScroller01.Classes.Objects.Items
{
    class DoorEnemySpawner : ActorSpawner
    {
        private readonly float FrameRate = 1f / 12;

        private const int DRAW_WIDTH = 66;
        private const int DRAW_HEIGHT = 144;

        private readonly float FromActorLayerDepth = 0.65f;

        public DoorEnemySpawner(Vector2 position, Level inLevel, Actor spawnedActor)
           : base(position, inLevel, spawnedActor)
        {
            this.Sprite = TextureManager.GetTexure("DoorEntry");
            this.CurrentFrameTime = 0;
            this.InitSpriteFrames(DRAW_WIDTH, DRAW_HEIGHT, 0, 0);
            this.SetOriginPosition("bottom");
            this.LayerDepth = 0.66f;
            this.SetDrawArea();
        }

        public override void Update(GameTime gT)
        {
            this.CurrentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;
            if (this.CurrentFrameTime >= FrameRate)
            {
                this.CurrentFrameTime = 0f;
                this.FrameX++;

                if (this.FrameY == 0)
                {
                    if (this.FrameX > 4)
                    {
                        this.FrameX = 0;
                        this.FrameY++;

                        SpawnActor();
                    }
                }
                else
                {
                    if (this.FrameX > 4)
                    {
                        this.FrameX = 0;
                        this.FrameY--;

                        this.Destroy();
                    }
                }

                this.SetDrawArea();
            }
        }

        protected override void SpawnActor()
        {
            // this.FromActor.Position = this.Position;
            InLevel.Actors.Add(this.FromActor);
        }

        public override void Draw(SpriteBatch SB)
        {
            SB.Draw(this.Sprite, Camera.GetScreenPosition(this.Position), this.DrawArea, Color.White, 0f, this.Origin, 1f, SpriteEffects.None, this.LayerDepth);

            // Draw Enemy
            if (this.FromActor != null)
            {
                this.FromActor.DrawInDoorway(SB, this.FromActorLayerDepth);
            }
        }

        public override void Destroy()
        {
            this.FromActor = null;

            base.Destroy();
        }
    }
}

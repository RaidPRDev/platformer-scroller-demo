using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SideScroller01.Classes.Scene;

namespace SideScroller01.Classes.Objects.Items
{
    class SpecialEffects : GameItem
    {
        private const int DRAW_SMALLSPARK_WIDTH = 71;
        private const int DRAW_SMALLSPARK_HEIGHT = 67;
        private const int PLAYER_HEIGHT = 100;

        public SpecialEffects(string id, Vector2 position, Level inLevel, Actor actor)
            : base(position, inLevel, actor)
        {
            Debug.WriteLine("position: '{0}'", position);
            Debug.WriteLine("Position: '{0}'", this.Position);

            switch (id)
            {
                case "smallspark": // Small Spark
                    this.Sprite = TextureManager.GetTexure("small-spark"); ;
                    break;

                case "bluespark": // Blue Spark
                    this.Sprite = TextureManager.GetTexure("small-spark"); 
                    break;
            }

            this.InitSpriteFrames(DRAW_SMALLSPARK_WIDTH, DRAW_SMALLSPARK_HEIGHT);
            
            // Place this vector based of the actor position
            this.SetOffsetPosition(new Vector2(0, PLAYER_HEIGHT));

            // Offset item position 
            if (this.FromActor.FacingDir == DirectionTarget.Left)
                this.Position = new Vector2(Position.X - Offset.X, Position.Y - Offset.Y);
            else
                this.Position = new Vector2(Position.X - Offset.X, Position.Y - Offset.Y);

            this.SetOriginPosition();
        }

        public SpecialEffects(string id, DirectionTarget facingDir, Level inLevel, TrashCan gameItem)
            : base(Vector2.Zero, inLevel, null)
        {
            switch (id)
            {
                case "trashcanhit": // TrashCan Blue Spark
                    this.Sprite = TextureManager.GetTexure("small-spark");
                    break;

            }

            // Place this vector based of the actor position

            this.InitSpriteFrames(DRAW_SMALLSPARK_WIDTH, DRAW_SMALLSPARK_HEIGHT);

            this.SetOffsetPosition(new Vector2(50, 20));
            
            if (facingDir == DirectionTarget.Left)
                this.Position = new Vector2(gameItem.Position.X - Offset.X, gameItem.Position.Y - Offset.Y);
            else
                this.Position = new Vector2(gameItem.Position.X + Offset.X, gameItem.Position.Y - Offset.Y);

            this.SetOriginPosition();
        }

        public override void Update(GameTime gT)
        {
            this.Animate(gT);

            // Which way we traveling
            if (this.FrameX > 8) // Going Left
            {
                this.Destroy();
                return;
            }
        }

        public override void Draw(SpriteBatch SB)
        {
            SB.Draw(this.Sprite, Camera.GetScreenPosition(this.Position), this.DrawArea, Color.White, 0f, this.Origin, 1f, SpriteEffects.None, this.LayerDepth);
        }

        #region Animations

        protected override void Animate(GameTime gT)
        {
            this.CurrentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;
            if (this.CurrentFrameTime >= Actor.FrameRate)
            {
                this.CurrentFrameTime = 0f;
                this.FrameX++;

                if (this.FrameX > 9)
                {
                    this.Destroy();
                    return;
                }

                this.SetDrawArea();
            }
        }

        #endregion

        public override void Destroy()
        {
            base.Destroy();
        }
    }
}

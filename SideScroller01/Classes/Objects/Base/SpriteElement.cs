
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SideScroller01.Classes.Objects.Base
{
    class SpriteElement
    {
        public Vector2 Position;
        protected Vector2 Origin;
        public float LayerDepth;
        public bool IsVisible;

        protected Texture2D Sprite;
        protected Dictionary<string, Texture2D> SpriteFrames;
        protected Rectangle DrawArea;
        protected Color DrawColor;
        protected int DrawWidth;
        protected int DrawHeight;
        protected int FrameX;
        protected int FrameY;
        protected float CurrentFrameTime;

        public static Vector2 Offset = Vector2.Zero;

        public SpriteElement(Vector2 position)
        {
            this.Position = position;

            this.Sprite = null;
            this.SpriteFrames = new Dictionary<string, Texture2D>();

            this.DrawArea = Rectangle.Empty;
            this.DrawColor = Color.White;
            this.DrawWidth = 0;
            this.DrawHeight = 0;
            this.FrameX = 0;
            this.FrameY = 0;
            this.CurrentFrameTime = 0;

            this.InitializeTextures();
        }

        protected virtual void InitializeTextures()
        {

        }

        protected virtual void InitSpriteFrames(int dWidth, int dHeight, int fX = 0, int fY = 1)
        {
            this.DrawWidth = dWidth;
            this.DrawHeight = dHeight;
            this.FrameX = fX;
            this.FrameY = fY;
        }

        protected virtual void SetDrawArea()
        {
            DrawArea = new Rectangle(FrameX * DrawWidth, FrameY * DrawHeight, DrawWidth, DrawHeight);
        }

        protected virtual void SetOriginPosition(string pivot = "middle", int offsetX = 0, int offsetY = 0)
        {
            if (pivot == "middle")
            {
                this.Origin = new Vector2((this.DrawWidth / 2) + offsetX, (this.DrawHeight / 2) + offsetY);
            }
            else if (pivot == "bottom")
            {
                this.Origin = new Vector2((this.DrawWidth / 2) + offsetX, this.DrawHeight + offsetY);
            }
        }

        public virtual void SetPosition(Vector2 position)
        {
            this.Position = position;

            GetLayerDepth(this.Position.Y);
        }

        protected virtual void SetOffsetPosition(Vector2 position)
        {
            Offset = position;
        }

        public virtual void GetLayerDepth(float yPos)
        {

        }

        public virtual void Draw(SpriteBatch SB)
        {
            
        }

        public virtual void Update(GameTime gT)
        {

        }

        protected virtual void Animate(GameTime gT)
        {

        }
        
        public virtual void Destroy()
        {
            Sprite = null;      // lose reference just in case
        }

    }
}

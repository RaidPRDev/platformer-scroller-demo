using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SideScroller01
{

    class SpecialEffects : GameItem
    {

        private const int DRAW_SMALLSPARK_WIDTH = 71;
        private const int DRAW_SMALLSPARK_HEIGHT = 67;
        private const int PLAYER_HEIGHT = 100;
        

        Texture2D texture;
        Rectangle drawArea;
        Actor player;
        DirectionTarget facing_dir;
        TrashCan gameItem;
        private Vector2 origin;
        string EffectsID;
        float currentFrameTime;
        int drawWidth;
        int drawHeight;
        int frameX;
        int frameY;
        public static Vector2 Offset = Vector2.Zero;

        public SpecialEffects(string fxID, Vector2 ePosition, Level inLevel, Actor p)
        {
            this.InLevel = inLevel;
            this.player = p;
            this.EffectsID = fxID;
            this.Position = ePosition;
            
            switch (EffectsID)
            {
                case "smallspark": // Small Spark
                    this.EffectsID = null;
                    this.texture = Game1.SprSmallSpark;
                    this.drawWidth = DRAW_SMALLSPARK_WIDTH;
                    this.drawHeight = DRAW_SMALLSPARK_HEIGHT;
                    this.frameX = 0;
                    this.frameY = 0;
                    Offset = new Vector2(0, PLAYER_HEIGHT); // Place this vector based of the actor position
                    if (player.FacingDir == DirectionTarget.Left)
                        this.Position = new Vector2(Position.X - Offset.X, Position.Y - Offset.Y);
                    else
                        this.Position = new Vector2(Position.X - Offset.X, Position.Y - Offset.Y);

                    this.origin = new Vector2(this.drawWidth / 2, this.drawHeight / 2);
                    break;

                case "bluespark": // Blue Spark
                    this.EffectsID = null;
                    this.texture = Game1.SprSmallSpark;
                    this.drawWidth = DRAW_SMALLSPARK_WIDTH;
                    this.drawHeight = DRAW_SMALLSPARK_HEIGHT;
                    this.frameX = 0;
                    this.frameY = 1;
                    Offset = new Vector2(0, PLAYER_HEIGHT); // Place this vector based of the actor position
                    if (player.FacingDir == DirectionTarget.Left)
                        this.Position = new Vector2(Position.X - Offset.X, Position.Y - Offset.Y);
                    else
                        this.Position = new Vector2(Position.X - Offset.X, Position.Y - Offset.Y);

                    this.origin = new Vector2(this.drawWidth / 2, this.drawHeight / 2);
                    break;

            }


        }

        public SpecialEffects(string fxID, DirectionTarget facing, Level inLevel, TrashCan gameItem)
        {
            this.InLevel = inLevel;
            this.EffectsID = fxID;
            this.facing_dir = facing;
            this.gameItem = gameItem;

            switch (EffectsID)
            {
 
                case "trashcanhit": // TrashCan Blue Spark
                    this.EffectsID = null;
                    this.texture = Game1.SprSmallSpark;
                    this.drawWidth = DRAW_SMALLSPARK_WIDTH;
                    this.drawHeight = DRAW_SMALLSPARK_HEIGHT;
                    this.frameX = 0;
                    this.frameY = 1;
                    Offset = new Vector2(50, 20); // Place this vector based of the actor position
                    if (facing_dir == DirectionTarget.Left)
                        this.Position = new Vector2(gameItem.Position.X - Offset.X, gameItem.Position.Y - Offset.Y);
                    else
                        this.Position = new Vector2(gameItem.Position.X + Offset.X, gameItem.Position.Y - Offset.Y);

                    this.origin = new Vector2(this.drawWidth / 2, this.drawHeight / 2);
                    break;

            }

        }

        public override void Update(GameTime gT)
        {

            AnimateEffect(gT);

            // Which way we traveling
            if (this.frameX > 8) // Going Left
            {
                InLevel.GameItems.Remove(this);
                return;
            }

            

        }
        public override void Draw(SpriteBatch SB)
        {
            //SB.Draw(this.texture, Camera.GetScreenPosition(this.Position), drawArea, Color.White);

            SB.Draw(this.texture, Camera.GetScreenPosition(this.Position),
            drawArea, Color.White, 0f, this.origin, 1f, SpriteEffects.None, this.LayerDepth);
        }


        #region Animations

        private void AnimateEffect(GameTime gT)
        {
            currentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;
            if (currentFrameTime >= Actor.FrameRate)
            {
                currentFrameTime = 0f;
                frameX++;

                if (frameX > 9)
                {
                    InLevel.GameItems.Remove(this);
                    return;
                }
                SetDrawArea();

            }
        }

        private void SetDrawArea()
        {
            drawArea = new Rectangle(frameX * drawWidth, frameY * drawHeight, drawWidth, drawHeight);
        }

        #endregion



    }
}

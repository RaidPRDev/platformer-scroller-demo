using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SideScroller01
{
    enum DirectionTarget
    {
        Left,
        Right,
        Neither
    }
    
    class Actor
    {
        #region draw area constants

        // WAIT - Not doing anything, show this one
        private const int DRAW_HEIGHT_WAIT = 122;
        private const int DRAW_WIDTH_WAIT = 96;

        private const int DRAW_HEIGHT_NORMAL = 122;
        private const int DRAW_WIDTH_IDLE = 96;
        private const int DRAW_WIDTH_WALK = 96;
        private const int DRAW_WIDTH_JUMP = 78;
        private const int DRAW_WIDTH_JUMPKICK = 137;

        private const int DRAW_HEIGHT_JUMP = 133;
        private const int DRAW_HEIGHT_JUMPKICK = 133;
        private const int DRAW_WIDTH_JUMPKICK2 = 137;
        private const int DRAW_HEIGHT_JUMPKICK2 = 133;
        private const int DRAW_WIDTH_JUMPPUNCH = 137;
        private const int DRAW_HEIGHT_JUMPPUNCH = 133;

        // Hit Game Item
        private const int DRAW_WIDTH_KICKITEM = 137;
        private const int DRAW_HEIGHT_KICKITEM = 136;

        // Pickup Game Item
        private const int DRAW_WIDTH_PICKUP = 139;
        private const int DRAW_HEIGHT_PICKUP = 133;

        // COMBO 01 - Punch - Punch - Punch
        private const int D_HEIGHT_COMBO1_ATTACK01 = 121;
        private const int D_HEIGHT_COMBO1_ATTACK02 = 121;
        private const int D_HEIGHT_COMBO1_ATTACK03 = 121;
        private const int D_WIDTH_COMBO1_ATTACK01 = 113;
        private const int D_WIDTH_COMBO1_ATTACK02 = 113;
        private const int D_WIDTH_COMBO1_ATTACK03 = 131;
        private const int D_ATTACK01_COMBO1_FRAME_Y = 0;
        private const int D_ATTACK02_COMBO1_FRAME_Y = 1;
        private const int D_ATTACK03_COMBO1_FRAME_Y = 3;

        // COMBO 02 - Kick - Dash Kick
        private const int D_HEIGHT_COMBO2_ATTACK01 = 136;
        private const int D_HEIGHT_COMBO2_ATTACK02 = 136;
        private const int D_HEIGHT_COMBO2_ATTACK03 = 136;
        private const int D_WIDTH_COMBO2_ATTACK01 = 131;
        private const int D_WIDTH_COMBO2_ATTACK02 = 131;
        private const int D_WIDTH_COMBO2_ATTACK03 = 131;
        private const int D_ATTACK01_COMBO2_FRAME_Y = 0;
        private const int D_ATTACK02_COMBO2_FRAME_Y = 0;
        private const int D_ATTACK03_COMBO2_FRAME_Y = 1;

        // Enemy
        private const int DRAW_WIDTH_TAKEHIT = 136;
        private const int DRAW_HEIGHT_TAKEHIT = 133;
        private const int DRAW_WIDTH_GETTINGUP = 137;
        private const int DRAW_WIDTH_KNOCKDOWN = 139;

        // Game Items
        private const int DRAW_WIDTH_ROCKTHROW = 139;

        #endregion

        #region other values

        const byte SHADOW_OPACITY = 60;
        public static float FrameRate = 1f / 12;

        public const int HIT_Y_RANGE = 15;
        public const float DOWN_TIME = 1f; // How long does the character stay kocked down, before he can get up 

        public float ENEMY_DEATH_FLASH_TIME = 0.2f;
        public float PLAYER_DEATH_FLASH_TIME = 0.2f;

        //public float KnockDownDistance;
        //public float KnockDownSpeed;

        #endregion

        public DirectionTarget FacingDir;

        public Level InLevel;
        public Vector2 Position;
        public float LayerDepth;
        Vector2 originShadow;

        // Collision Detection
        public bool IsAttackable;
        public bool IsAttacking;
        public int HitArea;

        // Health and Death
        public float Health;
        public int DeathFlashes;
        public int GettingUpFlashes;
        public bool IsVisible;

        public Actor(Vector2 position, Level InLevel)
        {
            this.Position = position;
            this.InLevel = InLevel;

            originShadow = new Vector2(Game1.SprCharacterShadow.Width / 2,
                                       Game1.SprCharacterShadow.Height);
            
            GetLayerDepth();
            this.IsVisible = true;

        }

        public virtual void Update(GameTime gT)
        {

        }

        public virtual void Draw(SpriteBatch SB)
        {
            SB.Draw(Game1.SprCharacterShadow, Camera.GetScreenPosition(this.Position),
                null, new Color(Color.White, SHADOW_OPACITY), 0f,
                originShadow, 1f, SpriteEffects.None, this.LayerDepth);

            //Draw Hit Area TESTING
            
          /*  if (FacingDir == Direction.Right)
                SB.Draw(Game1.SprSinglePixel, new Rectangle((int)this.Position.X, (int)this.Position.Y, 
                    HitArea, 30), new Color(Color.White, 80));
            else
                SB.Draw(Game1.SprSinglePixel, new Rectangle((int)this.Position.X - HitArea, 
                    (int)this.Position.Y , HitArea, 30), new Color(Color.White, 80));
            */
        }

        public virtual void DrawInDoorway(SpriteBatch SB, float layerDepth)
        {

        }


        public void GetLayerDepth()
        {
            // Get Actors position as a % of total play area
            int min = InLevel.PlayBounds.Top;
            int max = InLevel.PlayBounds.Bottom;
            int range = max - min;
            float percent = ((float)this.Position.Y - (float)min) / (float)range;

            percent = 1f - percent;

            // Convert % to a value of Layer Depth Range
            // Player LayerDepth section 0.4(front) - 0.6(back) - Range 0.2f
            this.LayerDepth = percent * 0.2f + 0.4f;


        }

        // Remove Actor from level
        public void RemoveActorFromLevel()
        {
            InLevel.Actors.Remove(this);
        }

        public bool CheckForDeath()
        {
            if (this.Health <= 0)
            {
                // Dead
                //SoundManager.PlaySound("GetHit-Died");
                return true;
            }
            // Not Dead yet
            return false;
        }

        #region Collision Detection

        public virtual void UpdateHitArea()
        {

        }

        public virtual void GetHit(DirectionTarget cameFrom, int damage)
        {
        }

        public virtual void GetHitKick(DirectionTarget cameFrom, int damage)
        {
        }

        public virtual void GetKnockedDown(DirectionTarget cameFrom, int damage)
        {
        }

        #endregion  


    }
}


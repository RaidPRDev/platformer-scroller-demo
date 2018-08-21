using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SideScroller01.Classes.Objects.Base;
using SideScroller01.Classes.Scene;

namespace SideScroller01.Classes.Objects
{
    enum DirectionTarget
    {
        Left,
        Right,
        Neither
    }
    
    class Actor : SpriteElement
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

        #region Textures 

        protected Texture2D Texture_Walk_Idle;
        protected Texture2D Texture_Attack;
        protected Texture2D Texture_Attack_02;
        protected Texture2D Texture_React;
        protected Texture2D Texture_SinglePixel;
        protected Texture2D Texture_Shadow;

        #endregion

        public DirectionTarget FacingDir;

        public Level InLevel;
        protected Vector2 originShadow;

        // Collision Detection
        public bool IsAttackable;
        public bool IsAttacking;
        public int HitArea;

        // Health and Death
        public float Health;
        public int DeathFlashes;
        public int GettingUpFlashes;

        protected Vector2 Speed;

        public Actor(Vector2 position, Level InLevel)
            : base(position)
        {
            this.InLevel = InLevel;

            this.GetLayerDepth(this.Position.Y);
            this.Speed = Vector2.Zero;
            this.IsVisible = true;
        }

        protected override void InitializeTextures()
        {
            Texture_Walk_Idle = null;
            Texture_Attack = null;
            Texture_Walk_Idle = null;
            Texture_Attack_02 = null;
            Texture_React = null;
            Texture_SinglePixel = null;

            Texture_Shadow = TextureManager.GetTexure("CodySpriteShadow");
            Texture_SinglePixel = TextureManager.GetTexure("singlePixel");

            originShadow = new Vector2(Texture_Shadow.Width / 2, Texture_Shadow.Height);
        }

        public override void Update(GameTime gT)
        {

        }

        public override void Draw(SpriteBatch SB)
        {
            SB.Draw(Texture_Shadow, Camera.GetScreenPosition(this.Position), null, new Color(Color.White, SHADOW_OPACITY), 0f,
                originShadow, 1f, SpriteEffects.None, this.LayerDepth);
        }

        public override void GetLayerDepth(float yPos)
        {
            // Get Actors position as a % of total play area
            int min = InLevel.PlayBounds.Top;
            int max = InLevel.PlayBounds.Bottom;
            int range = max - min;
            float percent = (yPos - (float)min) / (float)range;

            percent = 1f - percent;

            // Convert % to a value of Layer Depth Range
            // Player LayerDepth section 0.4(front) - 0.6(back) - Range 0.2f
            this.LayerDepth = percent * 0.2f + 0.4f;
        }

        public virtual void DrawInDoorway(SpriteBatch SB, float layerDepth)
        {

        }

        // Remove Actor from level
        public void RemoveActorFromLevel()
        {
            InLevel.Actors.Remove(this);

            this.Destroy();
        }

        public bool CheckForDeath()
        {
            return (this.Health <= 0);
        }

        #region Collision Detection

        public virtual void UpdateHitArea()
        {
            this.HitArea = this.DrawWidth / 2;
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

        public override void Destroy()
        {
            // lose texture references just in case
            Texture_Walk_Idle = null;
            Texture_Attack = null;
            Texture_Walk_Idle = null;
            Texture_Attack_02 = null;
            Texture_React = null;
            Texture_SinglePixel = null;
            Texture_Shadow = null;
            Texture_SinglePixel = null;

            base.Destroy();
        }
    }
}


using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SideScroller01
{
    enum PlayerState
    {
        // Take Hit Cycle
        TakeHit,
        TakeHitLowKick,
        KnockedDown,
        Down,
        GettingUp,
        Dying,
        
        // Movement & Attacks
        Idle,
        Walking,
        Jumping,
        JumpKick,
        JumpFlyingKick,
        JumpFlyingPunch,
        Combo1Attack01,
        Combo1Attack02,
        Combo1Attack03,
        Combo2Attack01,
        Combo2Attack02,
        Combo2Attack03,
        ThrowRock,
        KickObject,
        PickUpItem,

        Level01Intro
        
    }
        
    class Player : Actor
    {
        
        #region other values
        
        const float MOVEMENT_THRESHOLD = 0.01f;
        //const float MOVEMENT_THRESHOLD = 0.05f;

        public float FrameWalkingSpeed = 0.5f; 

        static float acceptCombo1Attack2Time = Actor.FrameRate * 3f;
        static float acceptCombo1Attack3Time = Actor.FrameRate * 4f;

        static float acceptCombo2Attack2Time = Actor.FrameRate * 5f;
        static float acceptCombo2Attack3Time = Actor.FrameRate * 6f;

        public const float STARTING_HEALTH = 300;

        #endregion

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
        private const int D_WIDTH_COMBO1_ATTACK01 = 131;
        private const int D_WIDTH_COMBO1_ATTACK02 = 131;
        private const int D_WIDTH_COMBO1_ATTACK03 = 131;
        private const int D_ATTACK01_COMBO1_FRAME_Y = 1;
        private const int D_ATTACK02_COMBO1_FRAME_Y = 0;
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

        PlayerIndex pIndex;

        // Texture Details

        Texture2D texture;
        Vector2 originCharacters;
        Rectangle drawArea;
        float currentFrameTime;
        int drawWidth;
        int drawHeight;
        int frameX;
        int frameY;

        //Movement
        public PlayerState State;
        Vector2 jumpingPos; // Can't use the normal position as this would also change the position of the shadow
        float landingHeight; // Need to know where the landing of the jump is
        Vector2 speed;

        // Attacks
        float stateTime;
        bool makeCombo1;
        bool makeCombo2;
        public bool CarryingRock;
        GameItem aboutToUse; // Need a reference so action on a GameItem can be delayed
                             // until the correct frame of animation.

        // Cutscene
        Vector2 targetPosition;
        
               
        public Player(Vector2 position, Level inLevel, PlayerIndex pIndex)
            : base(position, inLevel)
        {
            this.pIndex = pIndex;
            this.CarryingRock = false;
            ResetIdleGraphic();
            this.Health = STARTING_HEALTH;
        }
         
        public override void Update(GameTime gT)
        {
            
            switch (State)
            {
                #region Idle and Walking

                case PlayerState.Idle:
                case PlayerState.Walking:

                    #region Punch Attack
                    //if (InputHelper.WasKeyPressed(Keys.a))
                    if (InputHelper.WasButtonPressed(pIndex, Buttons.X)
                        || InputHelper.WasKeyPressed(Keys.NumPad7)
                        || InputHelper.WasKeyPressed(Keys.RightControl))
                    {
                        #region Any GameItems Here on Punch?

                        // Are we close enough to a GameItem to use?
                        for (int i = 0; i < InLevel.GameItems.Count; i++)
                        {
                            GameItem gameItem;

                            gameItem = InLevel.GameItems[i] as TrashCan;
                            if (gameItem != null)
                            {
                                if (gameItem.CheckCollision(this))
                                {
                                    // Kick It!
                                    // Setup animation
                                    this.aboutToUse = gameItem;
                                    this.State = PlayerState.KickObject;
                                    texture = Game1.SprCharacterAttacks02;
                                    currentFrameTime = 0f;
                                    frameX = 0;
                                    frameY = 2;
                                    drawWidth = DRAW_WIDTH_KICKITEM;
                                    drawHeight = DRAW_HEIGHT_KICKITEM;
                                    originCharacters = new Vector2(drawWidth / 2, drawHeight);
                                    SetDrawArea();
                                    return;
                                }
                            }
                            gameItem = InLevel.GameItems[i] as PickUpStone;
                            if (gameItem != null)
                            {
                                if (gameItem.CheckCollision(this))
                                {
                                    PickUpItem(gameItem);
                                    return;
                                }
                            }
                            gameItem = InLevel.GameItems[i] as PickUpHealthPack;
                            if (gameItem != null)
                            {
                                if (gameItem.CheckCollision(this))
                                {
                                    PickUpItem(gameItem);
                                    return;
                                }
                            }
                        }
                        #endregion                        
                        
                        Game1.cam.SetZoom(1.15f, 20);
                        Game1.bloom.SetBloom(1.5f, 15f);

                        // Setup Draw Area
                        texture = Game1.SprCharacterAttacks;
                        drawWidth = D_WIDTH_COMBO1_ATTACK01;
                        drawHeight = D_HEIGHT_COMBO1_ATTACK01;
                        frameX = 0;
                        frameY = D_ATTACK01_COMBO1_FRAME_Y;
                        SetDrawArea();
                        originCharacters = new Vector2((drawWidth / 2), drawHeight);

                        stateTime = 0f;
                        State = PlayerState.Combo1Attack01;
                        makeCombo1 = false;

                        // Play Sound
                        SoundManager.PlaySound("Combo1-Hit");
                        return;

                    }
                    #endregion

                    #region Kick Attack
                    if (InputHelper.WasButtonPressed(pIndex, Buttons.Y)
                        || InputHelper.WasKeyPressed(Keys.NumPad8)
                        || InputHelper.WasKeyPressed(Keys.RightAlt))
                    {
                        #region Any GameItems Here?

                        // Are we close enough to a TrashCan to kick?
                        for (int i = 0; i < InLevel.GameItems.Count; i++)
                        {
                            GameItem gameItem;

                            gameItem = InLevel.GameItems[i] as TrashCan;
                            if (gameItem != null)
                            {
                                if (gameItem.CheckCollision(this))
                                {
                                    // Kick It!
                                    // Setup animation
                                    this.aboutToUse = gameItem;
                                    this.State = PlayerState.KickObject;
                                    texture = Game1.SprCharacterAttacks02;
                                    currentFrameTime = 0f;
                                    frameX = 0;
                                    frameY = 2;
                                    drawWidth = DRAW_WIDTH_KICKITEM;
                                    drawHeight = DRAW_HEIGHT_KICKITEM;
                                    originCharacters = new Vector2(drawWidth / 2, drawHeight);
                                    SetDrawArea();
                                    return;
                                }
                            }
                        }
                        #endregion


                        // Setup Draw Area
                        texture = Game1.SprCharacterAttacks02;
                        drawWidth = D_WIDTH_COMBO2_ATTACK01;
                        drawHeight = D_HEIGHT_COMBO2_ATTACK01;
                        frameX = 0;
                        frameY = D_ATTACK01_COMBO2_FRAME_Y;
                        SetDrawArea();
                        if (FacingDir == DirectionTarget.Left)
                            //originCharacters = new Vector2((drawWidth / 2) + 18, drawHeight + 2);
                            originCharacters = new Vector2((drawWidth / 2), drawHeight);
                        else // Right
                            //originCharacters = new Vector2((drawWidth / 2) - 18, drawHeight + 2);
                            originCharacters = new Vector2((drawWidth / 2), drawHeight);

                        stateTime = 0f;
                        State = PlayerState.Combo2Attack01;
                        makeCombo2 = false;
                        // Play Sound
                        SoundManager.PlaySound("Attack04");
                        return;

                    }
                    #endregion

                    #region Jump
                    //if (InputHelper.WasKeyPressed(Keys.LeftShift))
                    if (InputHelper.WasButtonPressed(pIndex, Buttons.A)
                        || InputHelper.WasKeyPressed(Keys.NumPad4)
                        || InputHelper.WasKeyPressed(Keys.Space))
                    {
                        landingHeight = Position.Y;
                        jumpingPos = Position;
                        speed.Y = -7; // Jump Speed

                        State = PlayerState.Jumping;
                        texture = Game1.SprCharacterReact;
                        frameX = 0;
                        frameY = 0;
                        drawWidth = DRAW_WIDTH_JUMP;
                        drawHeight = DRAW_HEIGHT_JUMP;
                        originCharacters = new Vector2(drawWidth / 2, drawHeight);
                        SetDrawArea();
                        // Play Sound
                        SoundManager.PlaySound("Jump");
                        return;
                    }
                    #endregion

                    #region ThrowRocks

                    if (InputHelper.WasButtonPressed(this.pIndex, Buttons.B)
                        || InputHelper.WasKeyPressed(Keys.NumPad5)
                        || InputHelper.WasKeyPressed(Keys.Enter))
                    {
                        if (CarryingRock)
                        {
                            // Setup the animation
                            State = PlayerState.ThrowRock;
                            texture = Game1.SprCharacterReact;
                            frameX = 0;
                            frameY = 7;
                            currentFrameTime = 0f;
                            drawWidth = DRAW_WIDTH_ROCKTHROW;
                            drawHeight = DRAW_HEIGHT_TAKEHIT;
                            originCharacters = new Vector2((drawWidth / 2), (drawHeight));
                            SetDrawArea();
                            // Play Sound
                            SoundManager.PlaySound("Attack01");
                            return;
                        }
                    }

                    #endregion
                    
                    Move(gT);
                    break;

                #endregion

                #region Jumping

                case PlayerState.Jumping:
                    #region JumpKickAttack1
                    //if (InputHelper.WasKeyPressed(Keys.A))
                    if (InputHelper.WasButtonPressed(pIndex, Buttons.Y)
                        || InputHelper.WasKeyPressed(Keys.NumPad8)
                        || InputHelper.WasKeyPressed(Keys.RightAlt)) // Jump Kick
                    {
                       // texture = Game1.SprCharacterAttacks02;
                        State = PlayerState.JumpKick;
                        frameX = 0;
                        frameY = 2;
                        drawWidth = DRAW_WIDTH_JUMPKICK;
                        drawHeight = DRAW_HEIGHT_JUMPKICK;

                        originCharacters = new Vector2((drawWidth / 2), drawHeight);
                        // Play Sound
                        SoundManager.PlaySound("Attack02");

                        //return;
                    }
                    //if (InputHelper.WasKeyPressed(Keys.G))
                    else if (InputHelper.WasButtonPressed(pIndex, Buttons.B)
                        || InputHelper.WasKeyPressed(Keys.NumPad5)
                        || InputHelper.WasKeyPressed(Keys.Enter)) // Side Flying Kick
                    {
                        State = PlayerState.JumpFlyingKick;
                        frameX = 0;
                        frameY = 3;
                        //drawWidth = DRAW_WIDTH_JUMPKICK;
                        //drawHeight = DRAW_HEIGHT_JUMPKICK;
                        //drawWidth = 137;
                        //drawHeight = 133;
                        drawWidth = DRAW_WIDTH_JUMPKICK2;
                        drawHeight = DRAW_HEIGHT_JUMPKICK2;

                        originCharacters = new Vector2((drawWidth / 2), drawHeight);
                        // Play Sound
                        SoundManager.PlaySound("GetOutOfMyFace");
                        SoundManager.PlaySound("Attack03");

                        //return;
                    }
                    else if (InputHelper.WasButtonPressed(pIndex, Buttons.X)
                        || InputHelper.WasKeyPressed(Keys.NumPad7)) // Flying Jump Punch
                    {
                        State = PlayerState.JumpFlyingPunch;
                        frameX = 0;
                        frameY = 4;
                        //drawWidth = DRAW_WIDTH_JUMPKICK;
                        //drawHeight = DRAW_HEIGHT_JUMPKICK;
                        //drawWidth = 137;
                        //drawHeight = 133;
                        drawWidth = DRAW_WIDTH_JUMPPUNCH;
                        drawHeight = DRAW_HEIGHT_JUMPPUNCH;

                        originCharacters = new Vector2((drawWidth / 2), drawHeight);
                        // Play Sound
                        SoundManager.PlaySound("ThrowPunch");

                        //return;
                    }

                    #endregion

                    jumpingPos += speed;
                    Position.X = jumpingPos.X;
                    speed.Y += 0.3f; // Slow Verticle speed down because of gravity
                    if (jumpingPos.Y >= landingHeight) // Have we landed?
                    {
                        ResetIdleGraphic();
                        speed.Y = 0;
                        break;
                    }
                    ConstrainToScreen();
                    if (State == PlayerState.JumpKick)
                    {
                        AnimateJumpKick(gT);
                    }
                    else if (State == PlayerState.JumpFlyingKick)
                    {
                        AnimateFlyingJumpKick(gT);
                    }
                    else
                    {
                        AnimateJumping(gT);
                    }
                    
                    break;

                #endregion

                #region JumpKick

                case PlayerState.JumpKick:
                    jumpingPos += speed;
                    Position.X = jumpingPos.X;
                    speed.Y += 0.2f; // Slow Verticle speed down because of gravity when kicking
                    if (jumpingPos.Y >= landingHeight) // Have we landed?
                    {
                        ResetIdleGraphic();
                        speed.Y = 0;
                        break;
                    }
                    ConstrainToScreen();
                    AnimateJumpKick(gT);    
                    
                    break;

                #endregion

                #region JumpPunch

                case PlayerState.JumpFlyingPunch:
                    jumpingPos += speed;
                    Position.X = jumpingPos.X;
                    speed.Y += 0.2f; // Slow Verticle speed down because of gravity when kicking
                    if (jumpingPos.Y >= landingHeight) // Have we landed?
                    {
                        ResetIdleGraphic();
                        speed.Y = 0;
                        break;
                    }
                    ConstrainToScreen();
                    AnimateJumpPunch(gT);

                    break;

                #endregion

                #region JumpFlyingKick

                case PlayerState.JumpFlyingKick:
                    jumpingPos += speed;
                    Position.X = jumpingPos.X;
                    speed.Y += 0.2f; // Slow Verticle speed down because of gravity when kicking
                    if (jumpingPos.Y >= landingHeight) // Have we landed?
                    {
                        ResetIdleGraphic();
                        speed.Y = 0;
                        break;
                    }
                    ConstrainToScreen();
                    AnimateFlyingJumpKick(gT);

                    break;

                #endregion

                #region Combo1PunchAttack01
                case PlayerState.Combo1Attack01:

                    stateTime += (float)gT.ElapsedGameTime.TotalSeconds;
                    if (stateTime < acceptCombo1Attack2Time) // Still time to make a combo
                    {
                        //|| InputHelper.WasKeyPressed(Keys.A)
                        if (InputHelper.WasButtonPressed(pIndex, Buttons.X)
                            || InputHelper.WasKeyPressed(Keys.NumPad7)
                            || InputHelper.WasKeyPressed(Keys.RightControl))
                        {
                            makeCombo1 = true;
                            Game1.cam.SetZoom(1.25f, 20);
                            Game1.bloom.SetBloom(1.9f, 15f);

                        }
                        //else if (InputHelper.WasKeyPressed(Keys.D)) 
                        else if (InputHelper.WasButtonPressed(pIndex, Buttons.Y)
                            || InputHelper.WasKeyPressed(Keys.NumPad8)
                            || InputHelper.WasKeyPressed(Keys.RightAlt)
                            && frameX > 2)
                            // If Punching for Combo, Check to see if player pressed Kick button after
                            // If, so change animation to kick
                        {
                            makeCombo1 = false;

                            texture = Game1.SprCharacterAttacks02;
                            drawWidth = D_WIDTH_COMBO2_ATTACK01;
                            drawHeight = D_HEIGHT_COMBO2_ATTACK01;
                            frameX = 0;
                            frameY = D_ATTACK01_COMBO2_FRAME_Y;
                            SetDrawArea();
                            originCharacters = new Vector2((drawWidth / 2), drawHeight);

                            stateTime = 0f;
                            State = PlayerState.Combo2Attack01;
                            // Play Sound
                            SoundManager.PlaySound("Combo2-Hit");
                            
                        }
                    }

                    else // stateTime IS bigger than acceptAttack2Time
                    {
                        if (makeCombo1)
                        {
                            // Setup draw area
                            drawWidth = D_WIDTH_COMBO1_ATTACK02;
                            drawHeight = D_HEIGHT_COMBO1_ATTACK02;
                            frameX = 3;
                            frameY = D_ATTACK02_COMBO1_FRAME_Y;
                            originCharacters = new Vector2(drawWidth / 2, drawHeight);
                            SetDrawArea();

                            // Set Attack-Input times
                            stateTime = 0f;
                            State = PlayerState.Combo1Attack02;
                            makeCombo1 = false;

                            // Play Sound
                            SoundManager.PlaySound("TakeThat");
                            SoundManager.PlaySound("Combo2-Hit");

                            return;

                        }
                    }
                    AnimateCombo1Attack01(gT);
                    break;


                #endregion

                #region Combo1PunchAttack02
                case PlayerState.Combo1Attack02:

                    stateTime += (float)gT.ElapsedGameTime.TotalSeconds;
                    if (stateTime < acceptCombo1Attack3Time) // Still time to make a combo
                    {
                        //if (InputHelper.WasKeyPressed(Keys.A)) 
                        if (InputHelper.WasButtonPressed(pIndex, Buttons.X)
                            || InputHelper.WasKeyPressed(Keys.NumPad7)
                            || InputHelper.WasKeyPressed(Keys.RightControl))
                        {
                            makeCombo1 = true;
                            Game1.cam.SetZoom(1.45f, 20);
                            Game1.bloom.SetBloom(2.4f, 15f);

                        }
                    }

                    else // stateTime IS bigger than acceptAttack2Time
                    {
                        if (makeCombo1)
                        {
                            // Setup draw area
                            drawWidth = D_WIDTH_COMBO1_ATTACK03;
                            drawHeight = D_HEIGHT_COMBO1_ATTACK03;
                            frameX = 0;
                            frameY = D_ATTACK03_COMBO1_FRAME_Y;
                            originCharacters = new Vector2((drawWidth / 2), drawHeight);
                            SetDrawArea();

                            // Set Attack-Input times
                            stateTime = 0f;
                            State = PlayerState.Combo1Attack03;
                            makeCombo1 = false;
                            // Play Sound
                            SoundManager.PlaySound("Combo3-Hit");
                            return;

                        }
                    }
                    AnimateCombo1Attack02(gT);
                    break;


                #endregion

                #region Combo1PunchAttack03

                case PlayerState.Combo1Attack03:
                    AnimateCombo1Attack03(gT);
                    break;

                #endregion

                #region Combo2KickAttack01
                case PlayerState.Combo2Attack01:

                    stateTime += (float)gT.ElapsedGameTime.TotalSeconds;
                    if (stateTime < acceptCombo2Attack2Time) // Still time to make a combo
                    {
                        //if (InputHelper.WasKeyPressed(Keys.Z)) 
                        if (InputHelper.WasButtonPressed(pIndex, Buttons.Y)
                            || InputHelper.WasKeyPressed(Keys.NumPad8)
                            || InputHelper.WasKeyPressed(Keys.RightAlt))
                        {
                            makeCombo2 = true;
                        }
                        //else if (InputHelper.WasKeyPressed(Keys.D)) 
                        else if (InputHelper.WasButtonPressed(pIndex, Buttons.X)
                            || InputHelper.WasKeyPressed(Keys.NumPad7)
                            || InputHelper.WasKeyPressed(Keys.RightControl) 
                            && frameX >= 3)
                        // If Kicking for Combo, Check to see if player pressed Punch button after
                        // If, so change animation to punch
                        {
                            texture = Game1.SprCharacterAttacks;
                            drawWidth = D_WIDTH_COMBO1_ATTACK01;
                            drawHeight = D_HEIGHT_COMBO1_ATTACK01;
                            frameX = 0;
                            frameY = D_ATTACK01_COMBO1_FRAME_Y;
                            SetDrawArea();
                            if (FacingDir == DirectionTarget.Left)
                                originCharacters = new Vector2((drawWidth / 2) + 18, drawHeight + 2);
                            else // Right
                                originCharacters = new Vector2((drawWidth / 2) - 18, drawHeight + 2);

                            stateTime = 0f;
                            State = PlayerState.Combo1Attack01;
                            makeCombo1 = false;
                            // Play Sound
                            SoundManager.PlaySound("FinishHit");
                            return;

                        }

                    }

                    else // stateTime IS bigger than acceptAttack2Time
                    {
                        if (makeCombo2)
                        {
                            // Setup draw area
                            drawWidth = D_WIDTH_COMBO2_ATTACK02;
                            drawHeight = D_HEIGHT_COMBO2_ATTACK02;
                            frameX = 6;
                            frameY = D_ATTACK02_COMBO2_FRAME_Y;
                            originCharacters = new Vector2(drawWidth / 2, drawHeight);
                            SetDrawArea();

                            // Set Attack-Input times
                            stateTime = 0f;
                            State = PlayerState.Combo2Attack02;
                            makeCombo2 = false;
                            // Play Sound
                            SoundManager.PlaySound("Attack03");
                            SoundManager.PlaySound("FinishHit");
                            return;

                        }
                    }
                    AnimateCombo2Attack01(gT);
                    break;


                #endregion

                #region Combo2KickAttack02

                case PlayerState.Combo2Attack02:
                    AnimateCombo2Attack02(gT);
                    break;

                #endregion

                #region Rock Throw

                case PlayerState.ThrowRock:
                    AnimateThrowRock(gT);
                    break;

                #endregion

                #region TrashCan

                case PlayerState.KickObject:
                    AnimateKickObject(gT);
                    break;

                #endregion

                #region PickUpItems
                case PlayerState.PickUpItem:
                    AnimatePickUpItem(gT);
                    break;
                #endregion

                #region Take Hit and Die Cycle
                case PlayerState.TakeHit:
                    AnimateTakeHit(gT);
                    break;

                case PlayerState.KnockedDown:
                    AnimateKnockDown(gT);
                    break;

                case PlayerState.Down:
                    stateTime += (float)gT.ElapsedGameTime.TotalSeconds;
                    
                    if (stateTime >= Actor.DOWN_TIME)
                    {
                        // Set up Gettign Up Animation
                        State = PlayerState.GettingUp;
                        currentFrameTime = 0f;
                        frameX = 0;
                        frameY = 5;
                        drawWidth = DRAW_WIDTH_GETTINGUP;
                        drawHeight = DRAW_HEIGHT_TAKEHIT;
                        originCharacters = new Vector2(drawWidth / 2, drawHeight);
                        SetDrawArea();

                    }
                    break;

                case PlayerState.GettingUp:
                    //IsVisible = true;
                    stateTime -= (float)gT.ElapsedGameTime.TotalSeconds;
                    if (stateTime <= 0)
                    {

                        if (GettingUpFlashes <= 50)
                        {
                            stateTime = PLAYER_DEATH_FLASH_TIME;
                            IsVisible = !IsVisible;
                            GettingUpFlashes++;
                        }
                        
                    }
                    AnimateGettingUp(gT);
                    break;

                case PlayerState.Dying:
                    // Flash the Body a few times
                    stateTime -= (float)gT.ElapsedGameTime.TotalSeconds;
                    if (stateTime <= 0)
                    {
                        stateTime = PLAYER_DEATH_FLASH_TIME;
                        IsVisible = !IsVisible;
                        DeathFlashes++;

                        // Actor is Dead
                        if (DeathFlashes <= 8)
                        {
                            InLevel.LevelState = LevelState.Continue;
                            InLevel.TimerContinue = 11f - (float)gT.ElapsedGameTime.TotalSeconds;
                            HUDManager.Opacity = 0f;
                            
                        }
                        


                    }
                    break;

                #endregion

                #region Level Intro

                case PlayerState.Level01Intro:
                    if (this.Position != targetPosition)
                    {
                        // Move to the destination
                        LineUpXTargetPos();
                        LineUpYTargetPos();

                        AnimateWalking(gT);

                    }
                    else
                    {
                        ResetIdleGraphic();
                        InLevel.LevelState = LevelState.CutScene;
                        InLevel.CutScenes[InLevel.CurrentCutScene].PlayFirstLine();

                    }
                    break;


                #endregion


            }
        }

        public override void Draw(SpriteBatch SB)
        {

            // FANIA
            Vector2 pos = GameManager.Levels[GameManager.CurrentLevel].Player1.Position;

            if (this.IsVisible)
            {

                // Draw Character
                // Are we jumping
                if (State == PlayerState.Jumping || State == PlayerState.JumpKick
                    || State == PlayerState.JumpFlyingKick
                    || State == PlayerState.JumpFlyingPunch)
                {
                    // Facing Left or Right?
                    if (FacingDir == DirectionTarget.Right)
                        SB.Draw(texture, Camera.GetScreenPosition(jumpingPos), drawArea,
                            Color.White, 0f, originCharacters, 1f, SpriteEffects.None, LayerDepth);
                    else // We must be facing to the left
                        SB.Draw(texture, Camera.GetScreenPosition(jumpingPos), drawArea,
                            Color.White, 0f, originCharacters, 1f, SpriteEffects.FlipHorizontally, LayerDepth);

                }
                else
                {

                    // Facing Left or Right?
                    if (FacingDir == DirectionTarget.Right)
                        SB.Draw(texture, Camera.GetScreenPosition(Position), drawArea,
                            Color.White, 0f, originCharacters, 1f, SpriteEffects.None, LayerDepth);
                    else // We must be facing to the left
                        SB.Draw(texture, Camera.GetScreenPosition(Position), drawArea,
                            Color.White, 0f, originCharacters, 1f, SpriteEffects.FlipHorizontally, LayerDepth);

                }
                // Draw Shadow
            }
            base.Draw(SB);



        }

        private void GetXSpeed(float magnitude)
        {

            //float magnitude = InputHelper.NGS[(int)pIndex].ThumbSticks.Left.Length();

            if (magnitude <= 0.25f) /*sneak*/
            {
                FrameWalkingSpeed = 1.3f;
                if (FacingDir == DirectionTarget.Left)
                    speed.X = -0.2f;
                else
                    speed.X = 0.2f;

            }
            else if (magnitude <= 0.75f) /*walk*/
            {
                FrameWalkingSpeed = 0.8f;
                if (FacingDir == DirectionTarget.Left)
                    speed.X = -1f;
                else
                    speed.X = 1f;
            }
            else /*run*/
            {
                FrameWalkingSpeed = 0.5f;
                if (FacingDir == DirectionTarget.Left)
                    speed.X = -2.5f;
                else
                    speed.X = 2.5f;
            }

        }

        private void GetYSpeed(bool nDir, float magnitude)
        {

            //float magnitude = InputHelper.NGS[(int)pIndex].ThumbSticks.Left.Length();

            if (magnitude <= 0.25f) /*sneak*/
            {
                FrameWalkingSpeed = 1.3f;
                if (nDir)
                    speed.Y = -0.2f;
                else
                    speed.Y = 0.2f;

            }
            else if (magnitude <= 0.75f) /*walk*/
            {
                FrameWalkingSpeed = 0.8f;
                if (nDir)
                    speed.Y = -1f;
                else
                    speed.Y = 1f;
            }
            else /*run*/
            {
                FrameWalkingSpeed = 0.5f;
                if (nDir)
                    speed.Y = -2f;
                else
                    speed.Y = 2f;
            }

        }

        private void Move(GameTime gT)
        {
            FrameWalkingSpeed = 1f;
            #region X Movement
            if (InputHelper.IsKeyHeld(Keys.Left) || InputHelper.IsKeyHeld(Keys.A)
                || InputHelper.IsButtonHeld(pIndex, Buttons.DPadLeft))
            {
                FacingDir = DirectionTarget.Left;
               // float magnitude = InputHelper.NGS[(int)pIndex].ThumbSticks.Left.Length();
                GetXSpeed(0.99f);
                State = PlayerState.Walking;
                drawWidth = DRAW_WIDTH_WALK;
                drawHeight = DRAW_HEIGHT_NORMAL;
            }
            else if (InputHelper.NGS[(int)pIndex].ThumbSticks.Left.X < -MOVEMENT_THRESHOLD)
            {
                //speed.X = -3f;
                FacingDir = DirectionTarget.Left;
                GetXSpeed(InputHelper.NGS[(int)pIndex].ThumbSticks.Left.Length());
                State = PlayerState.Walking;
                drawWidth = DRAW_WIDTH_WALK;
                drawHeight = DRAW_HEIGHT_NORMAL;
            }
            else if (InputHelper.IsKeyHeld(Keys.Right) || InputHelper.IsKeyHeld(Keys.D)
                || InputHelper.IsButtonHeld(pIndex, Buttons.DPadRight))
            {
                FacingDir = DirectionTarget.Right;
                GetXSpeed(0.76f);
                State = PlayerState.Walking;
                drawWidth = DRAW_WIDTH_WALK;
                drawHeight = DRAW_HEIGHT_NORMAL;
            }

            else if (InputHelper.NGS[(int)pIndex].ThumbSticks.Left.X > MOVEMENT_THRESHOLD)
            {
                //speed.X = 3f;
                FacingDir = DirectionTarget.Right;
                GetXSpeed(InputHelper.NGS[(int)pIndex].ThumbSticks.Left.Length());
                State = PlayerState.Walking;
                drawWidth = DRAW_WIDTH_WALK;
                drawHeight = DRAW_HEIGHT_NORMAL;
            }
            else
            {
                speed.X = 0;
            }
            #endregion

            #region Y Movement
            if (InputHelper.IsKeyHeld(Keys.Down) || InputHelper.IsKeyHeld(Keys.S)
                || InputHelper.IsButtonHeld(pIndex, Buttons.DPadDown))
            {
                float magnitude = InputHelper.NGS[(int)pIndex].ThumbSticks.Left.Length();
                GetYSpeed(false, 0.99f);
                State = PlayerState.Walking;
                drawWidth = DRAW_WIDTH_WALK;
                drawHeight = DRAW_HEIGHT_NORMAL;
            }
            else if (InputHelper.NGS[(int)pIndex].ThumbSticks.Left.Y < -MOVEMENT_THRESHOLD)
            {
                //speed.Y = 2f;
                GetYSpeed(false, InputHelper.NGS[(int)pIndex].ThumbSticks.Left.Length());
                State = PlayerState.Walking;
                drawWidth = DRAW_WIDTH_WALK;
                drawHeight = DRAW_HEIGHT_NORMAL;
            }
            else if (InputHelper.IsKeyHeld(Keys.Up) || InputHelper.IsKeyHeld(Keys.W)
                || InputHelper.IsButtonHeld(pIndex, Buttons.DPadUp))
            {
                GetYSpeed(true, 0.99f);
                State = PlayerState.Walking;
                drawWidth = DRAW_WIDTH_WALK;
                drawHeight = DRAW_HEIGHT_NORMAL;
            }
            else if (InputHelper.NGS[(int)pIndex].ThumbSticks.Left.Y > MOVEMENT_THRESHOLD)
            {
                //speed.Y = 2f;
                GetYSpeed(true, InputHelper.NGS[(int)pIndex].ThumbSticks.Left.Length());
                State = PlayerState.Walking;
                drawWidth = DRAW_WIDTH_WALK;
                drawHeight = DRAW_HEIGHT_NORMAL;
            }
            else
            {
                speed.Y = 0;
            }
            #endregion

            if (speed == Vector2.Zero)
            {
                // Make Character AnimateIdle()
                frameY = 0;
                State = PlayerState.Idle;
                drawHeight = DRAW_HEIGHT_NORMAL;
                drawWidth = DRAW_WIDTH_IDLE;
                originCharacters = new Vector2(drawWidth / 2, drawHeight);
                AnimateIdle(gT);
            }
            else // that means we are moving
            {
                Position += speed;
                GetLayerDepth();
                ConstrainToScreen();
                AnimateWalking(gT);
            }
        }

        /// <summary>
        /// Does not let the player leave the screen
        /// </summary>
        private void ConstrainToScreen()
        {
            // Don't let the player out of the screen
            // Or out of playBounds
            if (Position.X < Camera.Position.X - Game1.SCREEN_WIDTH)
                Position.X = Camera.Position.X - Game1.SCREEN_WIDTH;

            if (Position.X > Camera.Position.X + Game1.SCREEN_WIDTH)
                Position.X = Camera.Position.X + Game1.SCREEN_WIDTH;

            if (Position.Y < InLevel.PlayBounds.Top)
                Position.Y = InLevel.PlayBounds.Top;

            if (Position.Y > InLevel.PlayBounds.Bottom)
                Position.Y = InLevel.PlayBounds.Bottom;

        }

        public void GetStone()
        {
            this.CarryingRock = true;
        }

        public void GetHealth(int howMuch)
        {
            this.Health += howMuch;
            if (this.Health > STARTING_HEALTH)
                this.Health = STARTING_HEALTH;

        }

        private void PickUpItem(GameItem gameItem)
        {
            // Pick It Up!
            // Setup animation
            this.aboutToUse = gameItem;
            this.State = PlayerState.PickUpItem;
            texture = Game1.SprCharacterReact;
            currentFrameTime = 0f;
            frameX = 0;
            frameY = 7;
            drawWidth = DRAW_WIDTH_PICKUP;
            drawHeight = DRAW_HEIGHT_PICKUP;
            originCharacters = new Vector2(drawWidth / 2, drawHeight);
            SetDrawArea();
            
        }

        private void ThrowRocks()
        {
            // Throw a rock 
            this.InLevel.GameItems.Add(new Rock(
                this.Position, this.FacingDir, this.InLevel, this));

            this.CarryingRock = false;

            // Play Rock Throw Sound
            SoundManager.PlaySound("TakeThat");


        }

        public void Continue()
        {
            this.Health = STARTING_HEALTH;

            // Set up the getting up animation
            this.State = PlayerState.GettingUp;
            texture = Game1.SprCharacterReact;
            drawWidth = DRAW_WIDTH_GETTINGUP;
            drawHeight = DRAW_HEIGHT_TAKEHIT;
            frameX = 0;
            frameY = 5;
            currentFrameTime = 0f;
            originCharacters = new Vector2(drawWidth / 2, drawHeight);
            SetDrawArea();

        }

        public void SetIntro01TargetPosition(Vector2 targetPosition)
        {

            this.targetPosition = targetPosition;
            this.State = PlayerState.Level01Intro;
            this.InLevel.LevelState = LevelState.Intro;
            // Setup Walk Animation
            this.drawWidth = DRAW_WIDTH_WALK;
            this.drawHeight = DRAW_HEIGHT_NORMAL;
            this.frameX = 0;
            this.frameY = 1;
            this.texture = Game1.SprCharacterWalkIdle;
            SetDrawArea();
        }

        private void LineUpXTargetPos  ()
        {
            // is the targetPosition to the left
            if (targetPosition.X < this.Position.X) // TargetPosition is to the left
            {
                // Move left
                this.Position.X -= 3f;
                FacingDir = DirectionTarget.Left;
                if (targetPosition.X >= this.Position.X)
                {
                    this.Position.X = targetPosition.X;
                }

            }
            else if (targetPosition.X > this.Position.X) // TargetPosition is to the right
            {
                // Move Right
                this.Position.X += 3f;
                FacingDir = DirectionTarget.Right;
                if (targetPosition.X <= this.Position.X)
                {
                    this.Position.X = targetPosition.X;
                }

            }
            

        }

        private void LineUpYTargetPos()
        {
            // is the targetPosition above or below
            if (targetPosition.Y < this.Position.Y) // TargetPosition is above
            {
                // Move Up
                this.Position.Y -= 2f;
                if (targetPosition.Y >= this.Position.Y)
                {
                    this.Position.Y = targetPosition.Y;
                }
            }
            else if (targetPosition.Y > this.Position.Y) // TargetPosition is below
            {
                // Move Down
                this.Position.Y += 2f;
                if (targetPosition.Y <= this.Position.Y)
                {
                    this.Position.Y = targetPosition.Y;
                }
            }


        }

        #region Collision Detection

        public override void UpdateHitArea()
        {
            HitArea = drawWidth / 2;
        }

        private void CheckEnemyCollision()
        {
            UpdateHitArea();

            for (int i = InLevel.Actors.Count - 1; i >= 0; i--)
            {
                Actor actor;

                // Make sure our not looking at ourself
                actor = InLevel.Actors[i] as Player;
                if (actor == this)
                    continue;

                // Are we facing ROLENTO enemy?
                actor = InLevel.Actors[i] as RolentoEnemy;
                if (actor != null)
                {
                    // Update the current actors Hit Area
                    actor.UpdateHitArea();

                    // 1) Is Actor/Enemy attackable?
                    if (actor.IsAttackable)
                    {
                        // 2) Are wer within Y Range
                        if (actor.Position.Y > this.Position.Y - HIT_Y_RANGE
                            && actor.Position.Y < this.Position.Y + HIT_Y_RANGE)
                        {
                            // 3) Which way is the player facing
                            if (this.FacingDir == DirectionTarget.Left)
                            {
                                // 4) Is the enemy/actor in front of us **LEFT**
                                if (actor.Position.X < this.Position.X)
                                {
                                    // 5) Players left edge <MORE LEFT> than actors RIGHT
                                    if (this.Position.X - HitArea < actor.Position.X + actor.HitArea)
                                    {
                                        // GREAT HIT THEM!
                                        HitSomeone(actor);

                                    }
                                }

                            }
                            //  3) Which way is the player facing
                            else
                            {
                                // A) is Actor in in front of us or RIGHT OF the player
                                if (actor.Position.X > this.Position.X)
                                {
                                    // 5) Players RIGHT EDGE is more right than actor's LEFT 
                                    if (this.Position.X + HitArea > actor.Position.X - actor.HitArea)
                                    {
                                        // GREAT HIT THEM!
                                        HitSomeone(actor);
                                    }

                                }
                            }
                        }
                    }

                }
                // Are we facing enemy?
                actor = InLevel.Actors[i] as DeejayEnemy;
                if (actor != null)
                {
                    // Update the current actors Hit Area
                    actor.UpdateHitArea();

                    // 1) Is Actor/Enemy attackable?
                    if (actor.IsAttackable)
                    {
                        // 2) Are wer within Y Range
                        if (actor.Position.Y > this.Position.Y - HIT_Y_RANGE
                            && actor.Position.Y < this.Position.Y + HIT_Y_RANGE)
                        {
                            // 3) Which way is the player facing
                            if (this.FacingDir == DirectionTarget.Left)
                            {
                                // 4) Is the enemy/actor in front of us **LEFT**
                                if (actor.Position.X < this.Position.X)
                                {
                                    // 5) Players left edge <MORE LEFT> than actors RIGHT
                                    if (this.Position.X - HitArea < actor.Position.X + actor.HitArea)
                                    {
                                        // GREAT HIT THEM!
                                        HitSomeone(actor);

                                    }
                                }

                            }
                            //  3) Which way is the player facing
                            else
                            {
                                // A) is Actor in in front of us or RIGHT OF the player
                                if (actor.Position.X > this.Position.X)
                                {
                                    // 5) Players RIGHT EDGE is more right than actor's LEFT 
                                    if (this.Position.X + HitArea > actor.Position.X - actor.HitArea)
                                    {
                                        // GREAT HIT THEM!
                                        HitSomeone(actor);
                                    }

                                }
                            }
                        }
                    }

                }
                // Are we facing enemy?
                actor = InLevel.Actors[i] as AdonEnemy;
                if (actor != null)
                {
                    // Update the current actors Hit Area
                    actor.UpdateHitArea();

                    // 1) Is Actor/Enemy attackable?
                    if (actor.IsAttackable)
                    {
                        // 2) Are wer within Y Range
                        if (actor.Position.Y > this.Position.Y - HIT_Y_RANGE
                            && actor.Position.Y < this.Position.Y + HIT_Y_RANGE)
                        {
                            // 3) Which way is the player facing
                            if (this.FacingDir == DirectionTarget.Left)
                            {
                                // 4) Is the enemy/actor in front of us **LEFT**
                                if (actor.Position.X < this.Position.X)
                                {
                                    // 5) Players left edge <MORE LEFT> than actors RIGHT
                                    if (this.Position.X - HitArea < actor.Position.X + actor.HitArea)
                                    {
                                        // GREAT HIT THEM!
                                        HitSomeone(actor);

                                    }
                                }

                            }
                            //  3) Which way is the player facing
                            else
                            {
                                // A) is Actor in in front of us or RIGHT OF the player
                                if (actor.Position.X > this.Position.X)
                                {
                                    // 5) Players RIGHT EDGE is more right than actor's LEFT 
                                    if (this.Position.X + HitArea > actor.Position.X - actor.HitArea)
                                    {
                                        // GREAT HIT THEM!
                                        HitSomeone(actor);
                                    }

                                }
                            }
                        }
                    }

                }
                #region Misc Enemies Backup
                /*
                // Are we facing enemy?
                actor = InLevel.Actors[i] as EnemyRanged;
                if (actor != null)
                {
                    // Update the current actors Hit Area
                    actor.UpdateHitArea();

                    // 1) Is Actor/Enemy attackable?
                    if (actor.IsAttackable)
                    {
                        // 2) Are wer within Y Range
                        if (actor.Position.Y > this.Position.Y - HIT_Y_RANGE
                            && actor.Position.Y < this.Position.Y + HIT_Y_RANGE)
                        {
                            // 3) Which way is the player facing
                            if (this.FacingDir == Direction.Left)
                            {
                                // 4) Is the enemy/actor in front of us **LEFT**
                                if (actor.Position.X < this.Position.X)
                                {
                                    // 5) Players left edge <MORE LEFT> than actors RIGHT
                                    if (this.Position.X - HitArea < actor.Position.X + actor.HitArea)
                                    {
                                        // GREAT HIT THEM!
                                        HitSomeone(actor);

                                    }
                                }

                            }
                            //  3) Which way is the player facing
                            else
                            {
                                // A) is Actor in in front of us or RIGHT OF the player
                                if (actor.Position.X > this.Position.X)
                                {
                                    // 5) Players RIGHT EDGE is more right than actor's LEFT 
                                    if (this.Position.X + HitArea > actor.Position.X - actor.HitArea)
                                    {
                                        // GREAT HIT THEM!
                                        HitSomeone(actor);
                                    }

                                }
                            }
                        }
                    }

                }
                // Are we facing enemy?
                actor = InLevel.Actors[i] as EnemyClose;
                if (actor != null)
                {
                    // Update the current actors Hit Area
                    actor.UpdateHitArea();

                    // 1) Is Actor/Enemy attackable?
                    if (actor.IsAttackable)
                    {
                        // 2) Are wer within Y Range
                        if (actor.Position.Y > this.Position.Y - HIT_Y_RANGE
                            && actor.Position.Y < this.Position.Y + HIT_Y_RANGE)
                        {
                            // 3) Which way is the player facing
                            if (this.FacingDir == Direction.Left)
                            {
                                // 4) Is the enemy/actor in front of us **LEFT**
                                if (actor.Position.X < this.Position.X) 
                                {
                                    // 5) Players left edge <MORE LEFT> than actors RIGHT
                                    if (this.Position.X - HitArea < actor.Position.X + actor.HitArea)
                                    {
                                        // GREAT HIT THEM!
                                        HitSomeone(actor);

                                    }
                                }

                            }
                            //  3) Which way is the player facing
                            else
                            {
                                // A) is Actor in in front of us or RIGHT OF the player
                                if (actor.Position.X > this.Position.X)
                                {
                                    // 5) Players RIGHT EDGE is more right than actor's LEFT 
                                    if (this.Position.X + HitArea > actor.Position.X - actor.HitArea)
                                    {
                                        // GREAT HIT THEM!
                                        HitSomeone(actor);
                                    }

                                }
                            }
                        }
                    }

                }*/
                #endregion
            }
        }

        private void SetEffect(string effect, Actor who, Player p) 
        {
            // Display Small Spark Effect on Hit
            SpecialEffects spark = new SpecialEffects(effect, who.Position, InLevel, p);
            InLevel.GameItems.Add(spark);  
        }

        private void HitSomeone(Actor whoToHit)
        {

            switch (State)
            {
                case PlayerState.Combo1Attack01: // Straight Jab
                    SetEffect("smallspark", whoToHit, this);
                    whoToHit.GetHit(FacingDir, 15);
                    break;
  
                case PlayerState.Combo1Attack02: // Upper Cut
                    SetEffect("bluespark", whoToHit, this);
                    whoToHit.GetHit(FacingDir, 25);
                    break;

                case PlayerState.Combo1Attack03: // Side Kick
                    SetEffect("smallspark", whoToHit, this);
                    whoToHit.GetKnockedDown(FacingDir, 35);
                    break;

                case PlayerState.Combo2Attack01: // Front Kick 
                    SetEffect("smallspark", whoToHit, this);
                    whoToHit.GetHitKick(FacingDir, 30);
                    break;

                case PlayerState.Combo2Attack02: // Super Slide Kick
                    SetEffect("bluespark", whoToHit, this);
                    whoToHit.GetKnockedDown(FacingDir, 50);
                    break;

                case PlayerState.JumpKick: // Super Slide Kick
                    SetEffect("smallspark", whoToHit, this);
                    whoToHit.GetHit(FacingDir, 50);
                    break;

                case PlayerState.JumpFlyingKick: // Super Slide Kick
                    SetEffect("bluespark", whoToHit, this);
                    whoToHit.GetKnockedDown(FacingDir, 50);
                    break;

                case PlayerState.JumpFlyingPunch: // Super Slide Punch
                    SetEffect("smallspark", whoToHit, this);
                    whoToHit.GetHit(FacingDir, 40);
                    break;
                    

            }

        }

        public override void GetHit(DirectionTarget cameFrom, int damage)
        {
            this.Health -= damage;
            if (CheckForDeath())
            {
                SoundManager.PlaySound("GetHit-Died");
                GetKnockedDown(cameFrom, 0);
                return;
            }

            // Set state and tetxture
            State = PlayerState.TakeHit;
            texture = Game1.SprCharacterReact;
            frameX = 0;
            frameY = 6;
            drawWidth = DRAW_WIDTH_TAKEHIT;
            drawHeight = DRAW_HEIGHT_TAKEHIT;
            originCharacters = new Vector2((drawWidth / 2), drawHeight);
            SetDrawArea();

            // Play Sound
            SoundManager.PlaySound("GetHit01");

            // Face the man who beat you
            if (cameFrom == DirectionTarget.Left)
                FacingDir = DirectionTarget.Right;
            else
                FacingDir = DirectionTarget.Left;


        }

        public override void GetKnockedDown(DirectionTarget cameFrom, int damage)
        {
            // Set state and texture
            IsAttackable = false;
            State = PlayerState.KnockedDown;
            texture = Game1.SprCharacterReact;
            frameX = 0;
            frameY = 1;
            drawWidth = DRAW_WIDTH_KNOCKDOWN;
            drawHeight = DRAW_HEIGHT_TAKEHIT;
            originCharacters = new Vector2((drawWidth / 2), drawHeight);
            SetDrawArea();

            // Play Sound
            SoundManager.PlaySound("GetHit03");

            // Set Bloom to Normal
            Game1.bloomIntensityIndex = BloomSettings.BloomIntensityNormal;

            // Face the man who beat you
            if (cameFrom == DirectionTarget.Right)
                FacingDir = DirectionTarget.Left;
            else
                FacingDir = DirectionTarget.Right;

            this.Health -= damage;
        }

        #endregion

        #region Animations

        /// <summary>
        /// Resets player to it's idle state, awaiting player input
        /// </summary>
        public void ResetIdleGraphic()
        {
            Game1.cam.SetZoom(1f, 7);
            Game1.bloomIntensityIndex = BloomSettings.BloomIntensityNormal;

            this.IsAttackable = true;
            texture = Game1.SprCharacterWalkIdle;
            State = PlayerState.Idle;

            currentFrameTime = 0f;
            frameX = 0;
            frameY = 0;
            drawWidth = DRAW_WIDTH_IDLE;
            drawHeight = DRAW_HEIGHT_NORMAL;
            SetDrawArea();
            originCharacters = new Vector2(drawWidth / 2, drawHeight);
            this.HitArea = drawWidth / 2;

        }

        private void SetDrawArea()
        {
            drawArea = new Rectangle(frameX * drawWidth, frameY * drawHeight, drawWidth, drawHeight);
        }
        
        private void AnimateIdle(GameTime gT)
        {
            currentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;// *.9f;
            if (currentFrameTime >= Actor.FrameRate * 0.8f)
            {
                currentFrameTime = 0f;
                frameX++;
                if (frameX > 7)
                    frameX = 0;

                SetDrawArea();

            }

        }

        private void AnimateWalking(GameTime gT)
        {
            originCharacters = new Vector2(drawWidth / 2, drawHeight);

            frameY = 1;
            currentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;

            if (currentFrameTime >= Actor.FrameRate * FrameWalkingSpeed)
            {
                currentFrameTime = 0;
                frameX++;

                if (frameY == 1)
                {
                    if (frameX > 9)
                    {
                        frameX = 0;
                        //frameY++;
                    }
                }
                else // FrameY must be equal to 2
                {
                    if (frameX > 9)
                    {
                        frameX = 0;
                        //frameY--;
                    }
                }
                SetDrawArea();

            }
 

        }

        private void AnimateJumping(GameTime gT)
        {
            currentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;
            if (currentFrameTime >= Actor.FrameRate)
            {
                currentFrameTime = 0f;
                frameX++;
                if (frameX > 7)
                {
                    frameX = 7;
                    return;
                }
                SetDrawArea();
            }
        }

        private void AnimateJumpKick(GameTime gT)
        {
            currentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;
            if (currentFrameTime >= Actor.FrameRate * 0.5f)
            {
                currentFrameTime = 0f;
                frameX++;
                if (frameX > 6)
                {
                    texture = Game1.SprCharacterReact;
                    State = PlayerState.Jumping;
                    drawHeight = DRAW_HEIGHT_JUMP;
                    drawWidth = DRAW_WIDTH_JUMP;
                    frameX = 1;
                    frameY = 0;
                    originCharacters = new Vector2(drawWidth / 2, drawHeight);
                }
                // Collision Detection
                if (frameX == 4) 
                    CheckEnemyCollision();

                SetDrawArea();
            }


        }

        private void AnimateJumpPunch(GameTime gT)
        {
            currentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;
            if (currentFrameTime >= Actor.FrameRate)
            {
                currentFrameTime = 0f;
                frameX++;
                if (frameX > 3)
                {
                    texture = Game1.SprCharacterReact;
                    State = PlayerState.Jumping;
                    drawHeight = DRAW_HEIGHT_JUMP;
                    drawWidth = DRAW_WIDTH_JUMP;
                    frameX = 1;
                    frameY = 0;
                    originCharacters = new Vector2(drawWidth / 2, drawHeight);
                }
                // Collision Detection
                if (frameX == 2)
                    CheckEnemyCollision();

                SetDrawArea();
            }


        }

        private void AnimateFlyingJumpKick(GameTime gT)
        {
            currentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;
            if (currentFrameTime >= Actor.FrameRate * 0.5f)
            {
                currentFrameTime = 0f;
                frameX++;
                if (frameX > 7)
                {
                    texture = Game1.SprCharacterReact;
                    State = PlayerState.Jumping;
                    drawHeight = DRAW_HEIGHT_JUMP;
                    drawWidth = DRAW_WIDTH_JUMP;
                    frameX = 1;
                    frameY = 0;
                    originCharacters = new Vector2(drawWidth / 2, drawHeight);
                }
                // Collision Detection
                if (frameX == 4)
                {
                    CheckEnemyCollision();
                }

                SetDrawArea();
            }


        }

        // Combo 1 Animations ( Punch - Upper Punch - Side Kick )
        private void AnimateCombo1Attack01(GameTime gT)
        {

            currentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds; // Speed
            if (currentFrameTime >= Actor.FrameRate)
            {
                currentFrameTime = 0f;
                frameX++;
                if (frameX > 3)
                {
                    ResetIdleGraphic();
                    return;
                }
                // Collision Detection
                if (frameX == 2)
                    CheckEnemyCollision();

                SetDrawArea();


            }

        }
        private void AnimateCombo1Attack02(GameTime gT)
        {
            currentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;
            if (currentFrameTime >= Actor.FrameRate * 0.8f)
            {
                currentFrameTime = 0f;
                frameX++;
                if (frameX > 8)
                {
                    ResetIdleGraphic();
                    return;
                }
                // Collision Detection
                if (frameX == 5)
                    CheckEnemyCollision();

                SetDrawArea();

            }
        }
        private void AnimateCombo1Attack03(GameTime gT)
        {
            currentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;
            if (currentFrameTime >= Actor.FrameRate)
            {
                currentFrameTime = 0f;
                frameX++;
                if (frameX > 6)
                {
                    ResetIdleGraphic();
                    return;
                }
                // Collision Detection
                if (frameX == 4)
                    CheckEnemyCollision();

                SetDrawArea();

            }
        }

        // Combo 2 Animations ( Kick - Super Kick )
        private void AnimateCombo2Attack01(GameTime gT)
        {

            currentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;// *1.4f; // Speed
            if (currentFrameTime >= Actor.FrameRate)
            {
                currentFrameTime = 0f;
                frameX++;
                if (frameX > 6)
                {
                    ResetIdleGraphic();
                    return;
                }
                // Collision Detection
                if (frameX == 4)
                    CheckEnemyCollision();

                SetDrawArea();

            }

        }
        private void AnimateCombo2Attack02(GameTime gT)
        {

            currentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;// *1.4f; // Speed
            if (currentFrameTime >= Actor.FrameRate)
            {
                currentFrameTime = 0f;
                frameX++;
                if (frameY == 0 && frameX > 6)
                {
                    frameX = 0;
                    frameY++;
                    return;
                }
                if (frameY == 1 && frameX > 6)
                {
                    ResetIdleGraphic();
                    return;
                }
                // Collision Detection
                if (frameY == 1 && frameX == 1)
                {
                    CheckEnemyCollision();
                }


                SetDrawArea();

            }

        }

        private void AnimateThrowRock(GameTime gT)
        {
            currentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;// *1.4f; // Speed
            if (currentFrameTime >= Actor.FrameRate)
            {
                currentFrameTime = 0f;
                frameX++;

                if (frameX > 7 && frameY == 7)
                {
                    frameX = 0;
                    frameY++;
                }
                if (frameX > 3 && frameY == 8)
                {
                    frameX = 0;
                    if (frameY == 8)
                    {
                        ResetIdleGraphic();
                        return;
                    }
                }
                // Do we throw rocks
                if (frameX == 0 && frameY == 8)
                {
                    ThrowRocks();
                }
                SetDrawArea();
            }

        }

        private void AnimateKickObject(GameTime gT)
        {
            currentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds; // Speed
            if (currentFrameTime >= Actor.FrameRate)
            {
                currentFrameTime = 0f;
                frameX++;

                // Finish animation
                if (frameX > 4)
                {
                    ResetIdleGraphic();
                    return;
                }
                // Correct frame to kick object
                if (frameX == 2)
                {
                    this.aboutToUse.TakeHit(this.FacingDir);
                    this.aboutToUse = null;

                }
                SetDrawArea();
            }

        }

        private void AnimatePickUpItem(GameTime gT)
        {
            currentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds; // Speed
            if (currentFrameTime >= Actor.FrameRate)
            {
                currentFrameTime = 0f;
                frameX++;

                // Finish animation
                if (frameX > 2)
                {
                    ResetIdleGraphic();
                    return;
                }
                // Correct frame to Pickup object
                if (frameX == 2)
                {
                    this.aboutToUse.GetPickedUp(this);
                    this.aboutToUse = null;

                }
                SetDrawArea();
            }
        }

        private void AnimateTakeHit(GameTime gT)
        {
            currentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;
            if (currentFrameTime >= Actor.FrameRate * 1.2f)
            {
                currentFrameTime = 0f;
                frameX++;

                //if (jumpingPos.Y >= landingHeight) // Have we landed?
                if (frameX > 1)
                {
                    ResetIdleGraphic();
                    return;
                }
                SetDrawArea();

            }
        }
       
        private void AnimateKnockDown(GameTime gT)
        {
            currentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;
            if (currentFrameTime >= Actor.FrameRate * 1.2f)
            {
                currentFrameTime = 0f;
                frameX++;
                if (frameX == 1 && CheckForDeath())
                {
                    SoundManager.PlaySound("GetHit-Died");
                }
                if (frameX > 7)
                {
                    if (CheckForDeath())
                    {

                        State = PlayerState.Dying;
                        stateTime = 1f;
                        frameX = 6;
                        return;
                    }
                    // Set state and texture
                    State = PlayerState.Down;
                    frameX = 6;
                    stateTime = 0f;
                    return;
                }
                SetDrawArea();

            }
        }
        private void AnimateGettingUp(GameTime gT)
        {
            currentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;
            float framePerSec = 1.5f;

            if (frameX == 7) framePerSec = 7f;
            else framePerSec = 1.5f;
            if (currentFrameTime >= Actor.FrameRate * framePerSec)
            {
                currentFrameTime = 0f;
                frameX++;
                if (frameX > 7)
                {
                    IsVisible = true;
                    ResetIdleGraphic();
                    return;
                }
                SetDrawArea();

            }
        }


        #endregion


    }
}


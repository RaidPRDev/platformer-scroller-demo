using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SideScroller01
{
    public class Camera
    {

        public float _zoom; // Camera Zoom
        public Matrix _transform; // Matrix Transform
        public Vector2 _pos; // Camera Position
        protected float _rotation; // Camera Rotation

        public static float MovementSpeed = 5.0f;
        public static float Elasticity = 0.005f;

        public static Vector2 Position;
        public static Vector2 GetScreenPosition(Vector2 worldPosition)
        {
            return worldPosition - Camera.Position +
                new Vector2(Game1.SCREEN_WIDTH / 2, Game1.SCREEN_HEIGHT / 2);

        }

        public Camera()
        {
            _zoom = 1.0f;
            _rotation = 0.0f;
            //.0_pos = Vector2.Zero;
            _pos.X = Game1.SCREEN_WIDTH / 2;
            _pos.Y = Game1.SCREEN_HEIGHT / 2;
        }

        // Sets and gets zoom
        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; } // if (_zoom > 1f) _zoom = 1f; } // Negative zoom will flip image
        }

        public void IncrementZoom(float amount, GameTime gT)
        {
            _zoom = _zoom + amount;// *(float)gT.ElapsedGameTime.TotalSeconds;
        }

        public void DecreaseZoom(float amount, GameTime gT)
        {
            _zoom = _zoom - amount;// *(float)gT.ElapsedGameTime.TotalSeconds;
            if (_zoom < 1f) _zoom = 1f; 
        }

        public void UpdateCameraZoom(float end, GameTime gT)
        {
            float calcZoom;

            if (_zoom < end) // Zooming In
            {
               
                Move(new Vector2(5, 2));
                calcZoom = (end - _zoom) * Camera.MovementSpeed * Camera.Elasticity;
                IncrementZoom(calcZoom, gT);

                if (_zoom > end)
                    _zoom = end;

            }
            else
            {
                calcZoom = (_zoom - end) * Camera.MovementSpeed * Camera.Elasticity;
                MoveToPosition(new Vector2(0, 2));
                DecreaseZoom(calcZoom, gT);

            }

        }

        public void SetZoom(float zoomin, int speed)
        {
            Game1._zoom = zoomin;
            Camera.MovementSpeed = speed;
        }

        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        // Auxiliary function to move the camera
        public void Move(Vector2 amount)
        {
            Vector2 p_pos = GameManager.Levels[GameManager.CurrentLevel].Player1.Position;
           
            if (_pos.Y < 380) 
                _pos.Y += amount.Y;

            if (_pos.X > Game1.SCREEN_WIDTH / 2)
                _pos.X -= amount.X;
            else if (_pos.X < Game1.SCREEN_WIDTH / 2)
                _pos.X += amount.X;


        }
        // Auxiliary function to move the camera
        public void MoveToPosition(Vector2 amount)
        {
            _pos.X = Game1.SCREEN_WIDTH / 2;

            if (_pos.Y != Game1.SCREEN_HEIGHT / 2)
            {
                if (_pos.Y < Game1.SCREEN_HEIGHT / 2)
                    _pos.Y += amount.Y;
                else if (_pos.Y > Game1.SCREEN_HEIGHT / 2)
                    _pos.Y -= amount.Y;
                else
                    _pos.Y = Game1.SCREEN_HEIGHT / 2;
            }
        }

        // Get set position
        public Vector2 Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }
        public Matrix get_transformation(GraphicsDevice graphicsDevice)
        {
            _transform =       // Thanks to o KB o for this solution
              Matrix.CreateTranslation(new Vector3(-_pos.X, -_pos.Y, 0)) *
                                         Matrix.CreateRotationZ(_rotation) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 0)) *
                                         Matrix.CreateTranslation(new Vector3(graphicsDevice.Viewport.Width * 0.5f, graphicsDevice.Viewport.Height * 0.5f, 0));


            return _transform;
        }

    }
}

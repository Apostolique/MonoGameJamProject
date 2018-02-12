﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace MonoGameJamProject
{
    /// <summary>
    /// Basic class for handling Input
    /// </summary>
    class Input
    {
        protected MouseState currentMouseState, previousMouseState;

        public void Update()
        {
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
        }

        public Vector2 MousePosition
        {
            get { return new Vector2(currentMouseState.X, currentMouseState.Y); }
        }

        public Point MouseGridPosition(int gridRatio)
        {
             return new Point(((int)Math.Floor(MousePosition.X / gridRatio)) , ((int)Math.Floor(MousePosition.Y / gridRatio)));
        }

        public bool MouseLeftButtonPressed
        {
            get { return currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released; }
        }

        public bool MouseRightButtonPressed
        {
            get { return currentMouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Released; }
        }
    }
}
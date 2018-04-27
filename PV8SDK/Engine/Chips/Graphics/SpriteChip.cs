﻿//   
// Copyright (c) Jesse Freeman. All rights reserved.  
//  
// Licensed under the Microsoft Public License (MS-PL) License. 
// See LICENSE file in the project root for full license information. 
// 
// Contributors
// --------------------------------------------------------
// This is the official list of Pixel Vision 8 contributors:
//  
// Jesse Freeman - @JesseFreeman
// Christer Kaitila - @McFunkypants
// Pedro Medeiros - @saint11
// Shawn Rakowski - @shwany

using System;
using System.Linq;
using PixelVisionSDK.Utils;

namespace PixelVisionSDK.Chips
{
    /// <summary>
    ///     The <see cref="SpriteChip" /> represents a way to store and retrieve
    ///     sprite pixel data. Internally, the pixel data is stored in a
    ///     <see cref="TextureData" /> class allowing you to dynamically resize
    ///     sprites at run time.
    /// </summary>
    public class SpriteChip : AbstractChip
    {
        protected int _colorsPerSprite = 8;
        
        /// <summary>
        ///     Sets the total number of sprite draw calls for the display.
        /// </summary>
        public int maxSpriteCount { get; set; }
        protected int _pages = 4;

        /// <summary>
        ///     Internal <see cref="TextureData" /> where sprites are stored
        /// </summary>
        protected TextureData _texture = new TextureData(128, 128);

        /// <summary>
        ///     Internal <see cref="cache" /> for faster lookup
        /// </summary>
        protected string[] cache;

        public int pageHeight = 128;

        //protected Vector2 pageSize = new Vector2(128, 128);
        public int pageWidth = 128;

//        protected int[][] pixelDataCache;
        private int tmpX;
        private int tmpY;

        public bool unique = true;

        /// <summary>
        ///     The global <see cref="width" /> of sprites in the engine. By default
        ///     this is set to 8.
        /// </summary>
        public int width { get; set; }

        /// <summary>
        ///     The global <see cref="width" /> of sprites in the engine. By default
        ///     this is set to 8.
        /// </summary>
        public int height { get; set; }

        /// <summary>
        ///     A public getter for the internal
        ///     TextureData. When requested, a clone of the <see cref="_texture" />
        ///     field is returned. This is expensive and only used for tools.
        /// </summary>
        public TextureData texture
        {
            get { return _texture; }
            set { SpriteChipUtil.CloneTextureData(value, _texture); } //TODO do we need this?
        }

        /// <summary>
        ///     Return's the <see cref="Sprite" /> Ram's internal
        ///     <see cref="texture" /> <see cref="width" />
        /// </summary>
        public int textureWidth
        {
            get { return _texture.width; }
        }

        /// <summary>
        ///     Return's the <see cref="Sprite" /> Ram's internal
        ///     <see cref="texture" /> <see cref="width" />
        /// </summary>
        public int textureHeight
        {
            get { return _texture.height; }
        }

        /// <summary>
        ///     The virtual number of pages of sprite memory the SpriteChip
        ///     has. Each page contains a max number of sprites.
        /// </summary>
        public int pages
        {
            get { return _pages; }
            set
            {
                if (_pages == value)
                    return;

                _pages = value.Clamp(1, 8);
                Resize(pageWidth, pageHeight * pages);
            }
        }

        /// <summary>
        ///     Number of sprites on each page of memory.
        /// </summary>
        public int spritesPerPage
        {
            get { return pageWidth / width * (pageHeight / height); }
        }

        /// <summary>
        ///     Total number of sprites that can be stored in memory.
        /// </summary>
        public int totalSprites
        {
            get { return spritesPerPage * pages; }
        }

        /// <summary>
        ///     Total number of sprites that exist in memory.
        /// </summary>
        public int spritesInRam
        {
            get { return cache.Count(x => x != null); }
        }

        /// <summary>
        ///     Number of colors per sprite.
        /// </summary>
        public int colorsPerSprite
        {
            get { return _colorsPerSprite; }
            set
            {
                // There can only be a minimum of 2 colors and a maximum of 16 colors
                _colorsPerSprite = value.Clamp(2, 16);
            }
        }

        /// <summary>
        ///     Tests to see if sprite <paramref name="data" /> is empty. This method
        ///     iterates over all the ints in the supplied <paramref name="data" />
        ///     array and looks for a value of -1. If all values are -1 then it
        ///     returns true.
        /// </summary>
        /// <param name="data">An array of ints</param>
        /// <returns>
        /// </returns>
        public bool IsEmpty(int[] data)
        {
            var total = data.Length;
            for (var i = 0; i < total; i++)
                if (data[i] > -1)
                    return false;

            return true;
        }

        /// <summary>
        ///     Returns the next empty id in the <see cref="cache" /> index. Used for
        ///     building tools to modify the <see cref="SpriteChip" /> like importers
        ///     or if you want to add sprite dynamically at runtime and need to know
        ///     the next open index.
        /// </summary>
        /// <returns>
        /// </returns>
        public int NextEmptyID()
        {
            return Array.FindIndex(cache, string.IsNullOrEmpty);
        }

        /// <summary>
        ///     This configures the <see cref="SpriteChip" /> when it is activated.
        ///     It registers itself with the engine as the default SpriteChip, it
        ///     calls Clear() to clear any data and also sets the defaut size to 8 x
        ///     8.
        /// </summary>
        public override void Configure()
        {
            engine.spriteChip = this;

            //_texture.wrapMode = false;
            width = 8;
            height = 8;

            Clear();
        }

        public override void Deactivate()
        {
            base.Deactivate();
            engine.spriteChip = null;
        }
        
        int totalSprites1;
        /// <summary>
        ///     This resizes the <see cref="Sprite" /> Ram's
        ///     internal TextureData. This will destroy all
        ///     sprites in memory.
        /// </summary>
        /// <param name="w">
        ///     New <see cref="width" /> for the internal
        ///     TextureData.
        /// </param>
        /// <param name="h">
        ///     New <see cref="height" /> for the internal
        ///     TextureData.
        /// </param>
        public void Resize(int w, int h)
        {
            var minW = width;
            var minH = height;

            w = Math.Max((int) Math.Ceiling((float) w / minW) * minW, minW);
            h = Math.Max((int) Math.Ceiling((float) h / minH) * minH, minH);

            if (textureWidth != w || textureHeight != h)
            {
                texture.Resize(w, h);
                Clear();
            }

            //TODO this needs to be double checked at different size sprites
            var cols = MathUtil.FloorToInt(textureWidth / width);
            var rows = MathUtil.FloorToInt(textureHeight / height);
            totalSprites1 = cols * rows;
        }

        /// <summary>
        ///     This caches a sprite for easier look up and duplicate detection.
        ///     Each sprite is cached as a string.
        /// </summary>
        /// <param name="index">
        ///     Index where the sprite's cached value should be stored in the
        ///     <see cref="cache" /> array.
        /// </param>
        /// <param name="data">
        ///     An array of ints that represents the sprite's color data.
        /// </param>
        private void CacheSprite(int index, int[] data)
        {
            cache[index] = SpriteChipUtil.SpriteDataToString(data);

            var totalPixels = width * height;

            var tmpPixels = new int[totalPixels];
            Array.Copy(data, tmpPixels, totalPixels);
//            pixelDataCache[index] = tmpPixels;
        }

        /// <summary>
        ///     Clears the <see cref="TextureData" /> to a default color index of -1
        ///     and also resets the cache. This removes all sprites from the chip.
        /// </summary>
        public void Clear()
        {
            cache = new string[totalSprites];
//            pixelDataCache = new int[totalSprites][];
            _texture.Clear();
        }

//        private int[] cachedSprite;
        private int totalSpritePixels;
//        private int[] tmpPixelData;

        private int width1;
        private int height1;
        private int w;
        
        /// <summary>
        ///     Returns an array of ints that represent a sprite. Each
        ///     int should be mapped to a color
        ///     <paramref name="index" /> to display in the renderer.
        /// </summary>
        /// <param name="index">
        ///     Anint representing the location in memory of the
        ///     sprite.
        /// </param>
        /// <param name="pixelData"></param>
        /// <returns>
        /// </returns>
        public void ReadSpriteAt(int index, int[] pixelData)
        {
            if (index == -1)
                return;

            width1 = _texture.width;
            height1 = _texture.height;

            w = width1 / width;

            tmpX = index % w * width;
            tmpY = index / w * height;
            
            // Flip y for Unity
//            tmpY = height1 - tmpY - height;

            _texture.CopyPixels(ref pixelData, tmpX, tmpY, width, height);

        }

        private int x;
        private int y;
        
        /// <summary>
        ///     Updates the sprite data at a given position in the
        ///     <see cref="texture" /> data.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pixels"></param>
        public void UpdateSpriteAt(int index, int[] pixels)
        {
            if (index < 0)
                return;

//            int spriteWidth = width;
//            int spriteHeight = height;
//            int x;
//            int y;
//            int index1 = index;
//            int width2 = _texture.width;
//            int height2 = _texture.height;
//            var totalSprites2 = SpriteChipUtil.CalculateTotalSprites(width2, height2, spriteWidth, spriteHeight);

            // Make sure we stay in bounds
            index = index.Clamp(0, totalSprites1 - 1);

            var w1 = _texture.width / width;

            x = index % w1 * width;
            y = index / w1 * height;

//            if (true)
//                y = _texture.height - y - height;

            _texture.SetPixels(x, y, width, height, pixels);
            
            CacheSprite(index, pixels);
        }

        /// <summary>
        ///     Finds a sprite by looking it up against the cache. Returns -1 if no
        ///     sprite is found. This is used for insuring duplicate sprites aren't
        ///     added to the TextureData.
        /// </summary>
        /// <param name="pixels">
        ///     An array of ints representing the sprite's color data.
        /// </param>
        /// <returns>
        /// </returns>
        public int FindSprite(int[] pixels, bool emptyCheck = false)
        {
            if (emptyCheck)
                if (IsEmpty(pixels))
                    return -1;

            var sprite = SpriteChipUtil.SpriteDataToString(pixels);

            return Array.IndexOf(cache, sprite);
        }
    }
}
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

namespace PixelVisionSDK
{

    public interface IKeyInput
    {

        /// <summary>
        ///     Returns the keyboard input entered in the this frame.
        /// </summary>
        /// <returns></returns>
        string ReadInputString();

        /// <summary>
        ///     <para>Returns true while the user holds down the key identified by the key KeyCode enum parameter.</para>
        /// </summary>
        /// <param name="key"></param>
        bool GetKey(int key);

//        bool ReadKey(int key);

        /// <summary>
        ///     <para>
        ///         Returns true during the frame the user starts pressing down the key identified by the key KeyCode enum
        ///         parameter.
        ///     </para>
        /// </summary>
        /// <param name="key"></param>
        bool GetKeyDown(int key);

        /// <summary>
        ///     <para>Returns true during the frame the user releases the key identified by name.</para>
        /// </summary>
        /// <param name="key"></param>
        bool GetKeyUp(int key);

        /// 
//        bool ReadKeyUp(int key);

        #region Deprecated

//        [Obsolete]
//        //string inputString { get; }
//        [Obsolete]

//        [Obsolete]
//        [Obsolete]

        #endregion

    }

}
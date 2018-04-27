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
using PixelVisionSDK.Utils;

namespace PixelVisionSDK
{

    /// <summary>
    ///     The AbstractData class represents a standard foundation for all
    ///     data objects in the engine. It implements the ISave, ILoad and
    ///     IInvalidate interfaces and provides as standard API for serializing
    ///     the data it contains via the CustomSerializeData() method.
    /// </summary>
    public abstract class AbstractData : IInvalidate
    {

        protected bool _invalid;

        /// <summary>
        ///     The invalid flag allows you to quickly see if data has been changed
        ///     on the data instance. This is used in conjunction with the
        ///     Invalidate() and ResetValidation() methods. Use this flag in classes
        ///     that have the potential to be expensive to update and need to be delayed
        ///     before refreshing their data.
        /// </summary>
        /// <value>Boolean</value>
        public virtual bool invalid
        {
            get { return _invalid; }
        }

        /// <summary>
        ///     This method allows a clean way to set the invalid property to true
        ///     signaling a change in the underlying data. This method could be overridden
        ///     to provide additional logic when the AbstractData is invalidated.
        /// </summary>
        public virtual void Invalidate()
        {
            _invalid = true;
        }

        /// <summary>
        ///     This method allows a clean way to reset the invalid property to false
        ///     signaling underlying data had finished updating. This method could be
        ///     overridden to provide additional logic when the AbstractData is
        ///     done changing.
        /// </summary>
        /// <param name="value"></param>
        public virtual void ResetValidation(int value = 0)
        {
            _invalid = Convert.ToBoolean(value.Clamp(0, 1));
        }

    }

}
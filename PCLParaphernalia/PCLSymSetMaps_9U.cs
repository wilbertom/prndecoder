﻿using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class defines a set of PCL Symbol Set map objects.
    /// 
    /// © Chris Hutchinson 2016
    /// 
    /// </summary>

    static partial class PCLSymSetMaps
    {
        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // u n i c o d e M a p _ 9 U                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Maps characters in symbol set to Unicode (UCS-2) code-points.      //
        //                                                                    //
        // ID       9U                                                        //
        // Kind1    309                                                       //
        // Name     Windows 3.0 Latin 1 (obsolete)                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void unicodeMap_9U()
        {
            const eSymSetMapId mapId = eSymSetMapId.map_9U;

            const Int32 rangeCt = 3;

            UInt16[][] rangeData = new UInt16[rangeCt][]
            {
                new UInt16 [2] {0x20, 0x7f},
                new UInt16 [2] {0x80, 0x9f},
                new UInt16 [2] {0xa0, 0xff}
            };

            UInt16[] rangeSizes = new UInt16[rangeCt];

            UInt16[][] mapDataPCL = new UInt16[rangeCt][];

            UInt16 rangeMin,
                   rangeMax,
                   rangeSize;

            //----------------------------------------------------------------//

            for (Int32 i = 0; i < rangeCt; i++)
            {
                rangeSizes[i] = (UInt16)(rangeData[i][1] -
                                           rangeData[i][0] + 1);
            }

            for (Int32 i = 0; i < rangeCt; i++)
            {
                mapDataPCL[i] = new UInt16[rangeSizes[i]];
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Range 0                                                        //
            //                                                                //
            //----------------------------------------------------------------//

            rangeMin = rangeData[0][0];
            rangeMax = rangeData[0][1];
            rangeSize = rangeSizes[0];

            for (UInt16 i = rangeMin; i <= rangeMax; i++)
            {
                mapDataPCL[0][i - rangeMin] = i;
            }

            mapDataPCL[0][0x5e - rangeMin] = 0x02c6;
            mapDataPCL[0][0x7e - rangeMin] = 0x02dc;
            mapDataPCL[0][0x7f - rangeMin] = 0x2592;

            //----------------------------------------------------------------//
            //                                                                //
            // Range 1                                                        //
            //                                                                //
            //----------------------------------------------------------------//

            rangeMin = rangeData[1][0];
            rangeMax = rangeData[1][1];
            rangeSize = rangeSizes[1];

            mapDataPCL[1][0x80 - rangeMin] = 0x20ac;
            mapDataPCL[1][0x81 - rangeMin] = 0xffff;    //<not a character> //
            mapDataPCL[1][0x82 - rangeMin] = 0xffff;    //<not a character> //
            mapDataPCL[1][0x83 - rangeMin] = 0xffff;    //<not a character> //
            mapDataPCL[1][0x84 - rangeMin] = 0xffff;    //<not a character> //
            mapDataPCL[1][0x85 - rangeMin] = 0xffff;    //<not a character> //
            mapDataPCL[1][0x86 - rangeMin] = 0xffff;    //<not a character> //
            mapDataPCL[1][0x87 - rangeMin] = 0xffff;    //<not a character> //
            mapDataPCL[1][0x88 - rangeMin] = 0xffff;    //<not a character> //
            mapDataPCL[1][0x89 - rangeMin] = 0xffff;    //<not a character> //
            mapDataPCL[1][0x8a - rangeMin] = 0xffff;    //<not a character> //
            mapDataPCL[1][0x8b - rangeMin] = 0xffff;    //<not a character> //
            mapDataPCL[1][0x8c - rangeMin] = 0xffff;    //<not a character> //
            mapDataPCL[1][0x8d - rangeMin] = 0xffff;    //<not a character> //
            mapDataPCL[1][0x8e - rangeMin] = 0xffff;    //<not a character> //
            mapDataPCL[1][0x8f - rangeMin] = 0xffff;    //<not a character> //

            mapDataPCL[1][0x90 - rangeMin] = 0xffff;    //<not a character> //
            mapDataPCL[1][0x91 - rangeMin] = 0x2018;
            mapDataPCL[1][0x92 - rangeMin] = 0x2019;
            mapDataPCL[1][0x93 - rangeMin] = 0xffff;    //<not a character> //
            mapDataPCL[1][0x94 - rangeMin] = 0xffff;    //<not a character> //
            mapDataPCL[1][0x95 - rangeMin] = 0xffff;    //<not a character> //
            mapDataPCL[1][0x96 - rangeMin] = 0xffff;    //<not a character> //
            mapDataPCL[1][0x97 - rangeMin] = 0xffff;    //<not a character> //
            mapDataPCL[1][0x98 - rangeMin] = 0xffff;    //<not a character> //
            mapDataPCL[1][0x99 - rangeMin] = 0xffff;    //<not a character> //
            mapDataPCL[1][0x9a - rangeMin] = 0xffff;    //<not a character> //
            mapDataPCL[1][0x9b - rangeMin] = 0xffff;    //<not a character> //
            mapDataPCL[1][0x9c - rangeMin] = 0xffff;    //<not a character> //
            mapDataPCL[1][0x9d - rangeMin] = 0xffff;    //<not a character> //
            mapDataPCL[1][0x9e - rangeMin] = 0xffff;    //<not a character> //
            mapDataPCL[1][0x9f - rangeMin] = 0xffff;    //<not a character> //

            //----------------------------------------------------------------//

            for (UInt16 i = 0; i < rangeSize; i++)
            {
                mapDataPCL[1][i] = mapDataPCL[1][i];
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Range 2                                                        //
            //                                                                //
            //----------------------------------------------------------------//

            rangeMin = rangeData[2][0];
            rangeMax = rangeData[2][1];
            rangeSize = rangeSizes[2];

            for (UInt16 i = rangeMin; i <= rangeMax; i++)
            {
                mapDataPCL[2][i - rangeMin] = i;
            }

            mapDataPCL[2][0xaf - rangeMin] = 0x02c9;

            //----------------------------------------------------------------//

            _sets.Add(new PCLSymSetMap(mapId,
                                         rangeCt,
                                         rangeData,
                                         null,
                                         mapDataPCL));
        }
    }
}
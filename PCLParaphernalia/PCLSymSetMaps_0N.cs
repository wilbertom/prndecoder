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
    /// © Chris Hutchinson 2015
    /// 
    /// </summary>

    static partial class PCLSymSetMaps
    {
        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // u n i c o d e M a p _ 0 N                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Maps characters in symbol set to Unicode (UCS-2) code-points.      //
        //                                                                    //
        // ID       0N                                                        //
        // Kind1    14                                                        //
        // Name     ISO 8859-1 Latin 1                                        //
        //          Western European                                          //
        //          Equivalent to Row 0 of Unicode Basic MultiLingual Plane   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void unicodeMap_0N()
        {
            const eSymSetMapId mapId = eSymSetMapId.map_0N;

            const Int32 rangeCt = 2;

            UInt16[][] rangeData = new UInt16[rangeCt][]
            {
                new UInt16 [2] {0x20, 0x7f},
                new UInt16 [2] {0xa0, 0xff}
            };

            UInt16[] rangeSizes = new UInt16[rangeCt];

            UInt16[][] mapDataStd = new UInt16[rangeCt][];
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
                mapDataStd[i] = new UInt16[rangeSizes[i]];
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
                mapDataStd[0][i - rangeMin] = i;
            }

            mapDataStd[0][0x7f - rangeMin] = 0xffff;    //<not a character> //

            //----------------------------------------------------------------//

            for (UInt16 i = 0; i < rangeSize; i++)
            {
                mapDataPCL[0][i] = mapDataStd[0][i];
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

            for (UInt16 i = rangeMin; i <= rangeMax; i++)
            {
                mapDataStd[1][i - rangeMin] = i;
            }

            //----------------------------------------------------------------//

            for (UInt16 i = 0; i < rangeSize; i++)
            {
                mapDataPCL[1][i] = mapDataStd[1][i];
            }

            mapDataPCL[1][0xaf - rangeMin] = 0x02c9;

            //----------------------------------------------------------------//

            _sets.Add(new PCLSymSetMap(mapId,
                                         rangeCt,
                                         rangeData,
                                         mapDataStd,
                                         mapDataPCL));
        }
    }
}
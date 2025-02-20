using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Controls;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// Class provides details of PCL XL Whitespace tags.
    ///
    /// � Chris Hutchinson 2010
    ///
    /// </summary>

    static class PCLXLWhitespaces
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static SortedList<Byte, PCLXLWhitespace> _tags =
            new SortedList<Byte, PCLXLWhitespace>();

        private static PCLXLWhitespace _tagUnknown;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L X L W h i t e s p a c e s                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        static PCLXLWhitespaces()
        {
            _tagUnknown = new PCLXLWhitespace(0x20, "??", "*** Unknown tag ***");

            populateTable();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h e c k T a g                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Searches the PCL XL Whitespace tag table for a matching entry.     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static Boolean checkTag(Byte tagToCheck,
                                        ref String mnemonic,
                                        ref String description)
        {
            Boolean seqKnown;

            PCLXLWhitespace tag;

            if (_tags.IndexOfKey(tagToCheck) != -1)
            {
                seqKnown = true;
                tag = _tags[tagToCheck];
            }
            else
            {
                seqKnown = false;
                tag = _tagUnknown;
            }

            mnemonic = tag.Mnemonic;
            description = tag.Description;

            return seqKnown;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d i s p l a y S t a t s C o u n t s                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Add counts of referenced sequences to nominated data table.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void displayStatsCounts(DataTable table,
                                               Boolean incUsedSeqsOnly,
                                               Boolean excUnusedResTags)
        {
            Int32 count = 0;

            Boolean displaySeq,
                    hddrWritten;

            DataRow row;

            hddrWritten = false;

            //----------------------------------------------------------------//

            displaySeq = true;

            count = _tagUnknown.StatsCtTotal;

            if (count == 0)
                displaySeq = false;

            if (displaySeq)
            {
                if (!hddrWritten)
                {
                    displayStatsCountsHddr(table);
                    hddrWritten = true;
                }

                row = table.NewRow();

                row[0] = _tagUnknown.Tag;
                row[1] = _tagUnknown.Mnemonic + ": " + _tagUnknown.Description;
                row[2] = _tagUnknown.StatsCtParent;
                row[3] = _tagUnknown.StatsCtChild;
                row[4] = _tagUnknown.StatsCtTotal;

                table.Rows.Add(row);
            }

            //----------------------------------------------------------------//

            foreach (KeyValuePair<Byte, PCLXLWhitespace> kvp in _tags)
            {
                displaySeq = true;

                count = kvp.Value.StatsCtTotal;

                if (count == 0)
                {
                    if (incUsedSeqsOnly)
                        displaySeq = false;
                }

                if (displaySeq)
                {
                    if (!hddrWritten)
                    {
                        displayStatsCountsHddr(table);
                        hddrWritten = true;
                    }

                    row = table.NewRow();

                    row[0] = kvp.Value.Tag;
                    row[1] = kvp.Value.Mnemonic + ": " + kvp.Value.Description;
                    row[2] = kvp.Value.StatsCtParent;
                    row[3] = kvp.Value.StatsCtChild;
                    row[4] = kvp.Value.StatsCtTotal;

                    table.Rows.Add(row);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d i s p l a y S t a t s C o u n t s H d d r                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Add statistics header lines to nominated data table.               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void displayStatsCountsHddr(DataTable table)
        {
            DataRow row;

            //----------------------------------------------------------------//

            row = table.NewRow();

            row[0] = "";
            row[1] = "_______________________";
            row[2] = "";
            row[3] = "";
            row[4] = "";

            table.Rows.Add(row);

            row = table.NewRow();

            row[0] = "";
            row[1] = "PCL XL Whitespace tags:";
            row[2] = "";
            row[3] = "";
            row[4] = "";

            table.Rows.Add(row);

            row = table.NewRow();

            row[0] = "";
            row[1] = "�����������������������";
            row[2] = "";
            row[3] = "";
            row[4] = "";

            table.Rows.Add(row);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i n c r e m e n t S t a t s C o u n t                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Increment the relevant statistics count for the DataType tag.      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void incrementStatsCount(Byte tagByte,
                                                Int32 level)
        {
            PCLXLWhitespace tag;

            if (_tags.IndexOfKey(tagByte) != -1)
                tag = _tags[tagByte];
            else
                tag = _tagUnknown;

            tag.incrementStatisticsCount(level);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i s K n o w n T a g                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Searches the PCL XL Whitespace tag table for a matching entry.     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static Boolean isKnownTag(Byte tagToCheck)
        {
            if (_tags.IndexOfKey(tagToCheck) != -1)
                return true;
            else
                return false;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p o p u l a t e T a b l e                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Populate the table of Whitespace tags.                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void populateTable()
        {
            addTag(0x00, "<NUL>", "Null");
            addTag(0x09, "<HT>", "Horizontal Tab");
            addTag(0x0a, "<LF>", "Line Feed");
            addTag(0x0b, "<VT>", "Vertical Tab");
            addTag(0x0c, "<FF>", "Form Feed");
            addTag(0x0d, "<CR>", "Carriage Return");
            addTag(0x20, "<SP>", "Space");
        }

        private static PCLXLWhitespace addTag(Byte tag, String mnemonic, String description)
        {
            PCLXLWhitespace ws = new PCLXLWhitespace(tag, mnemonic, description);
            _tags.Add(tag, ws);

            return ws;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        //  r e s e t S t a t s C o u n t s                                   //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Reset counts of referenced tags.                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void resetStatsCounts()
        {
            PCLXLWhitespace tag;

            _tagUnknown.resetStatistics();

            foreach (KeyValuePair<Byte, PCLXLWhitespace> kvp in _tags)
            {
                tag = kvp.Value;

                tag.resetStatistics();
            }
        }
    }
}

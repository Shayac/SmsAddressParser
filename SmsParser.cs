using System;
using System.Collections.Generic;
using System.Linq;

namespace GmailReceiver
{
    public class SmsParser
    {
        public Address GetAddress(string smsBody)
        {
            Address address = null;
            int x;
            string[] splitString = smsBody.Split(new char[] {' ', ',', '!', '(', ')', '?', '#', '&', '.', '-'},
                                                 StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < splitString.Length; i++)
            {
                splitString[i] = splitString[i].Trim(',', '!', '?','.','-','(',')','#');
            }


            //search for number
            for (int i = 0; i < splitString.Length; i++)
            {
                bool foundNumber = int.TryParse(splitString[i], out x);
                if (!foundNumber)
                {
                    continue;
                }

                // If number is found and string is not last in array
                if (i + 1 < splitString.Length)
                {
                    // If Street Direction is found
                    if (
                        StreetDirections.Any(
                            d => String.Equals(d, splitString[i + 1], StringComparison.OrdinalIgnoreCase)) &&
                        i + 1 < splitString.Length)
                    {
                        //i = HouseNumber
                        //[i+1] = Direction
                        //[i+2] = StreetNameWithoutSpace
                        //[i+3] ?= StreetType

                        // If Street Name w/o space is found
                        if (
                            i + 2 < splitString.Length &&
                            ValidStreetNamesWithOutSpace.Any(
                                n => String.Equals(n, splitString[i + 2], StringComparison.OrdinalIgnoreCase)))
                        {
                            address = new Address()
                                {
                                    HouseNumber = x,
                                    StreetDirection = splitString[i + 1],
                                    StreetName = splitString[i + 2]
                                };
                            if (i + 3 < splitString.Length &&
                                StreetTypes.Any(
                                    t => String.Equals(t, splitString[i + 3], StringComparison.OrdinalIgnoreCase)))
                            {
                                address.StreetType = splitString[i + 3];
                            }
                            break;
                        }
                            //If Street Name with space is found
                        else if (
                            //i = HouseNumber
                            //[i+1] = Direction
                            //[i+2] = StreetName1
                            //[i+3] = StreetName2
                            //[i+4] ?= StreetType
                            i + 3 < splitString.Length &&
                            StreetNamesWithSpace.Any(
                                n =>
                                String.Equals(n,
                                              String.Format(splitString[i + 2] + " " + splitString[i + 3],
                                                            StringComparison.OrdinalIgnoreCase))))
                        {
                            address = new Address()
                                {
                                    HouseNumber = x,
                                    StreetDirection = splitString[i + 1],
                                    StreetName = splitString[i + 2] + " " + splitString[i + 3]
                                };
                            if (i + 4 < splitString.Length &&
                                StreetTypes.Any(
                                    t => String.Equals(t, splitString[i + 4], StringComparison.OrdinalIgnoreCase)))
                            {
                                address.StreetType = splitString[i + 4];
                            }
                            break;
                        }
                    } 
                        // If no Street Direction is found
                    else if (
                        //i = HouseNumber
                        //[i+1] = StreetName1
                        //[i+2] ?= StreetType
                        i + 1 < splitString.Length &&
                        ValidStreetNamesWithOutSpace.Any(
                            n => String.Equals(n, splitString[i + 1], StringComparison.OrdinalIgnoreCase)))
                    {
                        address = new Address()
                            {
                                HouseNumber = x,
                                StreetName = splitString[i + 1],
                            };
                        if (i + 2 < splitString.Length &&
                            StreetTypes.Any(
                                t => String.Equals(t, splitString[i + 2], StringComparison.OrdinalIgnoreCase)))
                        {
                            address.StreetType = splitString[i + 2];
                        }
                        break;
                    }
                    else if (
                        //i = HouseNumber
                        //[i+1] = StreetName1
                        //[i+2] = StreetName2
                        //[i+3] ?= StreetType
                        i + 2 < splitString.Length &&
                        StreetNamesWithSpace.Any(
                            n => String.Equals(n, String.Format(splitString[i + 1] + " " + splitString[i + 2]))))

                    {
                        address = new Address()
                            {
                                HouseNumber = x,
                                StreetName = splitString[i + 1] + " " + splitString[i + 2],
                            };
                        if (i + 3 < splitString.Length &&
                            StreetTypes.Any(
                                t => String.Equals(t, splitString[i + 3], StringComparison.OrdinalIgnoreCase)))
                        {
                            address.StreetType = splitString[i + 3];
                        }
                        break;
                    }
                }
            }

            return address;
        }

        public IEnumerable<string> StreetDirections { get; set; }
        public IEnumerable<string> ValidStreetNamesWithOutSpace { get; set; }
        public IEnumerable<string> StreetNamesWithSpace { get; set; }
        public IEnumerable<string> StreetTypes { get; set; }
    }
}
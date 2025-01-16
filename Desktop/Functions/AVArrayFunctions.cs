using System;
using System.Diagnostics;
using System.Linq;

namespace ArnoldVinkCode
{
    public partial class AVArrayFunctions
    {
        //Create array with default objects
        public static T[] CreateArray<T>(int arrayLength, T arrayValue)
        {
            try
            {
                T[] newArray = new T[arrayLength];
                Array.Fill(newArray, arrayValue);
                return newArray;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to create object array: " + ex.Message);
                return null;
            }
        }

        //Reset array with default objects
        public static bool ResetArray<T>(T[] targetArray, T arrayValue)
        {
            try
            {
                Array.Fill(targetArray, arrayValue);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to reset object array: " + ex.Message);
                return false;
            }
        }

        //Clear array with null objects
        public static bool ClearArray<T>(T[] targetArray)
        {
            try
            {
                Array.Clear(targetArray);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to clear array: " + ex.Message);
                return false;
            }
        }

        //Count array objects that are not null
        public static int CountNotNull<T>(T[] targetArray)
        {
            try
            {
                return targetArray.Count(x => x != null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to count objects in array: " + ex.Message);
                return 0;
            }
        }

        //Insert object to beginning of array
        /// <summary>
        /// Note: This removes the last item in array
        /// </summary>
        public static bool InsertObjectBegin<T>(T[] targetArray, T arrayObject)
        {
            try
            {
                Array.Copy(targetArray, 0, targetArray, 1, targetArray.Length - 1);
                targetArray[0] = arrayObject;
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to insert object to begin of array: " + ex.Message);
                return false;
            }
        }

        //Insert object to ending of array
        /// <summary>
        /// Note: This removes the first item in array
        /// </summary>
        public static bool InsertObjectEnd<T>(T[] targetArray, T arrayObject)
        {
            try
            {
                int targetArrayLength = targetArray.Length - 1;
                Array.Copy(targetArray, 1, targetArray, 0, targetArrayLength);
                targetArray[targetArrayLength] = arrayObject;
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to insert object to end of array: " + ex.Message);
                return false;
            }
        }

        //Clone object array
        public static T[] CloneObjectArray<T>(T[] targetArray)
        {
            try
            {
                int targetArrayLength = targetArray.Length;
                T[] newArray = new T[targetArrayLength];
                Array.Copy(targetArray, 0, newArray, 0, targetArrayLength);
                return newArray;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to clone object array: " + ex.Message);
                return null;
            }
        }

        //Move object in array to left
        public static void MoveObjectInArrayLeft<T>(T[] targetArray, int moveIndex, int newIndex)
        {
            try
            {
                T MoveValue = targetArray[moveIndex];
                Array.Copy(targetArray, moveIndex + 1, targetArray, moveIndex, targetArray.Length - 1 - moveIndex);
                targetArray[newIndex] = MoveValue;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to move object in array left: " + ex.Message);
            }
        }

        //Move object in array to right
        public static void MoveObjectInArrayRight<T>(T[] targetArray, int moveIndex, int newIndex)
        {
            try
            {
                T MoveValue = targetArray[moveIndex];
                Array.Copy(targetArray, newIndex, targetArray, newIndex + 1, targetArray.Length - 1 - newIndex);
                targetArray[newIndex] = MoveValue;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to move object in array right: " + ex.Message);
            }
        }
    }
}
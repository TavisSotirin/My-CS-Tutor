using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// linked list of usable memory
// (HEAP) RESERVED BOX A [Arbitrary offset for heap] -> USED(MEM_LOC, SIZE) [Any node should have a location in memory and a size. Display will be built by filling in free nodes in between] -> RESERVED BOX B [Arbitrary offset between heap and stack]
// (STACK) RESERVED BOX B [Arbitrary offset between heap and stack] -> USED(MEM_LOC, SIZE) [contiguous] -> RESERVED BOX C [Arbitrary offset for stack
public class MemoryList
{
    
}
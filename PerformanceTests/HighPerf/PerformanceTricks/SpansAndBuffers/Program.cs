Console.WriteLine("");
// // See https://aka.ms/new-console-template for more information
//
// using System.Buffers;
// using System.Runtime.CompilerServices;
// using System.Runtime.InteropServices;
//
// // LearnAboutMemoryOwners();
// LearnAboutBuffers();
//
// void LearnAboutBuffers()
// {
//     string source = "hello";
//     string target = "world";
//
//     unsafe
//     {
//         Buffer.MemoryCopy(&source, &target, 1, 1);
//         Console.WriteLine(target);
//     }
// }
//
// void LearnAboutMemoryOwners()
// {
//     string text = "this is some random text";
//     using IMemoryOwner<char> owner = MemoryPool<char>.Shared.Rent(text.Length);
//     List<int> someList = [1, 2, 3, 4];
//     text.CopyTo(owner.Memory.Span);
//     
//     Memory<char> slice = owner.Memory.Slice(0, 4);
//     
//     foreach (char c in slice.Span)
//     {
//         Console.WriteLine(c);
//     }
// }
//
// void LearnAboutSpans()
// {
//     string text = "some text";
//
//     char pinnable = text.GetPinnableReference();
//     
//     unsafe
//     {
//         Span<char> span = stackalloc char[text.Length];
//
//         text.CopyTo(span);
//
//         char* address = &pinnable;
//         // Unsafe.Add()
//         //     MemoryMarshal.as
//         Console.WriteLine($"address of pinnable {*address}");
//         var a = span;
//         foreach (char c in span)
//         {
//             Console.Write(c);
//         }
//
//         CanIUseASpanWhichIsOnStack(span);
//     }
//
//     void CanIUseASpanWhichIsOnStack(Span<char> span)
//     {
//         Console.WriteLine("entering the method:");
//         foreach (char c in span)
//         {
//             Console.Write(c);
//         }
//     }
// }
//
//
// Console.WriteLine("Hello, World!");
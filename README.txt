========================================================================
=== asmz12 - Primitive assembler for the Z12 demonstration processor ===
========================================================================

The Z12 demonstration processor is a simple, easy-to-understand, MIPS-inspired
project that is not intended to perform serious work or be physically
implemented. It is designed as a teaching aid for the fundamentals of digital
logic and the register-register (or load-store) microarchitecture scheme.
Its functions have been fully implemented in Logisim, although the actual
layout is inefficient and makes no attempt to exploit pipelining or conform
with transistor-level standards. The reader may, if so inclined, attempt to
resolve these issues as an exercise.

The Z12 processor has 12-bit wide instruction words, works with 12-bit wide registers
and can directly access 4096 12-bit memory words. There are three types of
instructions, Register-type, Immediate-type, and Jump-type:

	- Register-type instructions are layed out as:

	  xxxx xx xx xx xx

	  where the first four bits (x) are opcode, the next two are the destination
	  register, the next two are operand register 1, and the next two are
	  operand register 2. The final two bits are unused, but could be made
	  into a function field if the user was so inclined.
	  KEEP IN MIND that the SRL and SRA instructions MUST be written with
	  three register fields even though only two are used, or the assembler
	  will fail. If you need to shift left, just add a register into itself
	  like so: ADD r0 r0 r0 (r0 now contains twice its own value).

	- Immediate-type instructions are layed out as:

	  xxxx xx xx xxxx

	  where the first four are opcode, the next two destination or source,
	  the next two source, and the final four immediate operand. Given that
	  a signed four-bit field can only represent -8 to 7, seperate instructions
	  are used for adding or subtracting an immediate, and LW and SW are
	  unsigned.
	  THIS MEANS that ADDI can only add up to 15 to a register, SUBI can only take
	  up to 15 from a register, SW and LW can only reference 16 locations on a base
	  register, and these locations are ABOVE the register, not around it.

	- Jump-type instructions are layed out as:

	  xxxx xxxxxxxx

	  where the first four are opcode and the final eight are a signed jump amount.
	  Given an eight-bit signed jump amount, a jump can be 128 instructions back
	  or 127 instructions forward.

The files included with this readme are:

	- The Z12 Logisim .circ file;
	- The Z12 Assembler program (written in C# and compiled with Roslyn - 
	  which should run on recent Windows installations);
	- A testing file (Z12_STRLITERAL) which loads a character array into memory
	  and serves as an example;
	- An output.txt file containing the result of assembling the Z12_STRLITERAL
	  file, and which shows Logisim ROM/RAM file format;
	- A batch file that will bring up a command prompt in the
	  working directory.

To use the assembler, navigate to whichever directory it is stored in with
a command prompt instance and type:

	.\asmz12 file.txt

where "file" is the name of your file. By default, the output is written 
to the standard output instead of straight to file; this is because an 
error in a source file only halts the assembler ON THE LINE AT WHICH IT
OCCURRED. If an error occurs, you will see output similar to:

	203
	212
	000
	ERROR: There is an invalid opcode in the source file.

For this reason, it is recommended that you run the assembler at least once
before writing to file. In order to write to file, redirect the standard
output as in the following:

	.\asmz12 file.txt > outfile.txt

VALID OPCODES ARE LISTED HERE, IN BINARY ASCENDING ORDER

ADD r1 r0 r0    # Add r0 with r0, and place the result in r1
SUB r1 r0 r0    # Subtract r0 from r0
ADDI r1 r0 1    # Add the immediate value '1' with r0 and place result in r1
SUBI r1 r0 1    # Subtract the immediate value '1' from r0
AND r2 r0 r1    # logical AND of r0 and r1
OR r2 r0 r1     # logical OR of r0 and r1
XOR r2 r0 r1    # logical XOR of r0 and r1
NOR r2 r0 r1    # logical NOR of r0 and r1
LW r0 r3 8      # Load the value in memory referenced by base-r3 and offset-8 into r0
SW r0 r3 8      # Store contents of r0 into memory referenced by base-r3 and offset-8
SRL r0 r0 r0    # Shift r0 right once logically and put into r0 (third r0 is a dummy slot)
SRA r0 r0 r0    # Shift r0 right once arithmetically and put into r0
JUMP 54         # Jump forward 54 instructions
BEQF r0 r1 13   # Branch forward 13 instructions if r0 is equal to r1
BEQB r0 r1 13   # Branch backward 13 instructions if r0 is equal to r1
BLB r0 r1 13    # Branch backward 13 instructions if r0 is less than r1
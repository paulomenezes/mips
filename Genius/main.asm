.data

	memory: .word 0x10010000

	win:	.asciiz "Voce ganhou\n"
	lose:	.asciiz "Voce perdeu\n"

.text
	
	li 	$t2, 0		# display count
	li	$t3, 256	# display total
	
	li 	$t5, 0		# game loop count
	li	$t6, 1		# current game loop
	li	$s7, 1		# index to sort new color
	
	li	$t7, 1		# GREEN
	li	$t8, 2		# BLUE
	li	$t9, 3		# GREY
				# 0 IS RED
	
	li	$s5, 22		# set max loop 
	
	addi	$sp, $sp, -80	# set stack to support 20 bytes (20 * 4)
	addi 	$s0, $sp, 0	# copy sp to $s0

	j	afterDelay	# dont delay when start
	
repeatGame:

	li	$v0, 32		# delay 
	li	$a0, 1000	# 1000 miliseconds
	syscall

afterDelay:
	beq	$t6, $t5, prepareKeyboard	# verify if the game ends
	addi	$t5, $t5, 1	# increase game loop
	
	li	$t2, 0		# reset display counter	
	la	$t4, memory	# get memory and save in $t5
	
	beq	$t5, $s7, sortNewColor
	lw	$a0, 0($s0)
	addi	$a0, $a0, -48

	j 	afterColor

sortNewColor:		
	li 	$a1, 4  	# set $a1 to the max bound.
    	li 	$v0, 42  	# generates the random number.
    	syscall

afterColor:	
	bne	$a0, $zero, other	# if random number ($a0) isn't zero, go to other color

	li	$s1, '0'		# copy a to $s1
	sw	$s1, 0($s0)		# copy $s1 to stack
	
	addi	$s0, $s0, 4		# increase stack offset
	
	li 	$t1, 0xFF0000		# put red color in $t1
	j 	after			# jump set color

other:	# if isn't red, verify if is green
	bne	$a0, $t7, noGreen	# if isn't green go to other color
	
	li	$s1, '1'		# copy b to $s1
	sw	$s1, 0($s0)		# copy $s1 to stack
	
	addi	$s0, $s0, 4		# increase stack offset
	
	li	$t1, 0x00FF00		# if is, set green
	j	after			# jump set color
	
noGreen: # if isn't red and green, verify if is blue
	bne	$a0, $t8, noBlue	# if isn't blue go to other color
	
	li	$s1, '2'		# copy c to $s1
	sw	$s1, 0($s0)		# copy $s1 to stack
	
	addi	$s0, $s0, 4		# increase stack offset
	
	li	$t1, 0x0000FF		# if is, set blue
	j	after			# jump set color
	
noBlue: # if isn't red, green and blue, set grey	
	li	$s1, '3'		# copy d to $s1
	sw	$s1, 0($s0)		# copy $s1 to stack
	
	addi	$s0, $s0, 4		# increase stack offset
	
	li	$t1, 0xDCDCDC		# set grey
        	        	        	
loop:	
	beq	$t3, $t2, drawBlackSettings	# compare if display draw ends
	addi 	$t2, $t2, 1			# increase display counter
	
after:	
	sw 	$t1, 0($t4)	# put $t1 in RAM for display	
	addi	$t4, $t4, 4	# add + 4 in $t4
	
	j loop

drawBlackSettings:	
	li	$v0, 32		# delay 
	li	$a0, 500	# 1000 miliseconds
	syscall

	li	$t2, 0		# reset display counter	
	
	la	$t4, memory	# get memory and save in $t5

drawBlack:	
	beq	$t3, $t2, repeatGame	# compare if display draw ends
	
	li	$t1, 0x000000		# set grey
	
	sw 	$t1, 0($t4)	# put $t1 in RAM for display	
	addi	$t4, $t4, 4	# add + 4 in $t4
	
	addi 	$t2, $t2, 1			# increase display counter
	
	j drawBlack

prepareKeyboard:
	li 	$t5, 0		# reset game count to start keyboard count
	addi	$s0, $sp, 0	# reset stack offset

listenKeyboard:
	beq	$t6, $t5, startGame	# verify if the game ends
	addi	$t5, $t5, 1	# increase game loop
	
	li   	$v0, 12		# read char   
  	syscall            
  	
	addiu 	$a0, $v0, 0	# save in $a0
	
	lw	$s6, 0($s0)	# save stack head to $s6
	bne	$s6, $a0, gameover	# compare if $s6 is read char, if isn't game over

  	addi	$s0, $s0, 4		# increase stack offset
  
	j	listenKeyboard

startGame:
	beq	$t5, $s5, gamewin	# very if game ends
	
	li 	$t5, 0		# reset game count to start new game loop
	
	addi	$t6, $t6, 1	# increase game loop total
	addi	$s7, $s7, 1
		
	addi	$s0, $sp, 0	# reset stack offset
	
	j repeatGame		# back to new game loop

gameover:
	li	$v0, 4		# print lose
	la	$a0, lose
	syscall
	
	li $v0, 31		# set midi 
	la $a0,	40		# set music
	la $a1, 1000		# time 
	la $a2, 7		# instrument
	la $a3, 100		# sound 
	syscall			# play
	
	j exit

gamewin:
	li	$v0, 4		# print win
	la	$a0, win
	syscall
	
	li $v0, 31		# set midi 
	la $a0,	60		# set music
	la $a1, 1000		# time 
	la $a2, 7		# instrument
	la $a3, 100		# sound 
	syscall			# play

exit:    	
    	li 	$v0, 10		# set up exit syscall
	syscall
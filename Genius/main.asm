.data

memory: .word 0x10010000
array:	.space 22

.text
	li 	$t2, 0		# display count
	li	$t3, 256	# display total
	
	li 	$t5, 0		# game loop count
	li	$t6, 8		# current game loop
	
	li	$t7, 1		# GREEN
	li	$t8, 2		# BLUE
	li	$t9, 3		# GREY
				# 0 IS RED
	
	j	afterDelay	# dont delay when start
	
repeatGame:

	li	$v0, 32		# delay 
	li	$a0, 1000	# 1000 miliseconds
	syscall

afterDelay:
	beq	$t6, $t5, prepareKeyboard	# verify if the game ends
	addi	$t5, $t5, 4	# increase game loop
	
	li	$t2, 0		# reset display counter	
	la	$t4, memory	# get memory and save in $t5
		
	li 	$a1, 4  	# set $a1 to the max bound.
    	li 	$v0, 42  	# generates the random number.
    	syscall
        	        	        	
loop:	
	beq	$t3, $t2, repeatGame	# compare if display draw ends
	addi 	$t2, $t2, 1		# increase display counter
	
	bne	$a0, $zero, other	# if random number ($a0) isn't zero, go to other color
	
	la	$s0, 'a'
	sw 	$s0, array($t5)
	
	li 	$t1, 0xFF0000		# put red color in $t1
	j 	after			# jump set color

other:	# if isn't red, verify if is green
	bne	$a0, $t7, noGreen	# if isn't green go to other color
	
	la	$s0, 'b'
	sw 	$s0, array($t5)
	
	li	$t1, 0x00FF00		# if is, set green
	j	after			# jump set color
	
noGreen: # if isn't red and green, verify if is blue
	bne	$a0, $t8, noBlue	# if isn't blue go to other color
	
	la	$s0, 'c'
	sw 	$s0, array($t5)
	
	li	$t1, 0x0000FF		# if is, set blue
	j	after			# jump set color
	
noBlue: # if isn't red, green and blue, set grey
	la	$s0, 'd'
	sw 	$s0, array($t5)
	
	li	$t1, 0xDCDCDC		# set grey

after:	
	sw 	$t1, 0($t4)	# put $t1 in RAM for display	
	addi	$t4, $t4, 4	# add + 4 in $t4
	
	j loop

prepareKeyboard:
	li 	$t5, 0	

listenKeyboard:
	beq	$t6, $t5, exit	# verify if the game ends
	addi	$t5, $t5, 4	# increase game loop
	
	li   	$v0, 12		# read char   
  	syscall            
  	
	addiu $a0, $v0, 0	# save in $a0

  	li   $v0, 11       	# print char
  	syscall
  
  	lw	$a0, array($t5)
  
  	li   $v0, 11       	# print char
  	syscall
  
	j	listenKeyboard

exit:    	
    	li 	$v0, 10		# set up exit syscall
	syscall
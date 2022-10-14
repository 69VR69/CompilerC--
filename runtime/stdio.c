int print (int n) {
	
	int[] digits;
    int i;
	i = 0;
    
    while (n > 0)
    {
        digits[i] = n % 10;
        n = n / 10;
        i = i + 1;
    }
       
	int j;     
    for (j = i - 1; j >= 0; j = j - 1)
    {
        send(digits[j] + 48);
		
    }
	
	send(10);
}
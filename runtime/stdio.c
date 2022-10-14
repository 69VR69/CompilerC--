int print (int n) {
	
	int[] digits = new int[10];
    int i = 0;
    
    while (n > 0)
    {
        digits[i] = n % 10;
        n /= 10;
        i++;
    }
            
    for (int j = i - 1; j >= 0; j--)
    {
        int ascii = digits[j] + 48;
        Console.Write((char)ascii);
    }
}
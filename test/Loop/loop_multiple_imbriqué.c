int main() {
	int a;
	int b;
	int c;
	a = 2;
	c = 3;
	
	while (a < 3)
	{
		a = 1 + a;
		for (b = 2; b < 3; b = 1 + b)
		{
			c = 1 + (2 * b) + a;
		}
	}
	
	do {
		c = 1 + c;
	} while (c < 3);
}

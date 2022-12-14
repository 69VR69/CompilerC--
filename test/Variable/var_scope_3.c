int test(int a)
{
	int b;
	b = 4;
	a = 1;
	{
		int a;
		a = 2;
		int c;
		c = 3;
		b = 5;
	}
	int d;
	return a;
}

int main() {
	int a;
	a = 8;
	
	a = test(a);

	a = a + 2;
	//b = b + 2; //Should fail on this line due to b being out of scope
	//c = c + 2; //Should fail on this line due to c being out of scope
	//d = d + 2; //Should fail on this line due to d being out of scope
}

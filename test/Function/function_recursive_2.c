int test(int a, int b)
{
	if(a > b)
		return 3;
		
	return test (test(a+10,b)+10, b) + 10;
}

int main() {
	int a;
	a = 5;
	a = test(a, 10);
}
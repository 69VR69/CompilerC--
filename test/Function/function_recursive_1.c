int test(int a, int b)
{
	if(a > b)
		return 3;
	else
		return test (a+b,b);
}

int main() {
	int a;
	a = 5;
	a = test(a, 10);
}
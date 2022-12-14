int main() {
	int a;
	a = 8;
	
	for (a = 0; a < 10; a = a + 1) {
		int a;
		a = 9;
		int b;
		b = 1;
	}

	a = a + 2;
	//b = b + 2; //Should fail on this line due to b being out of scope
}

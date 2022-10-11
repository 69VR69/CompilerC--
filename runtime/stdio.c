int print (int n) {
	if(n < 10){
		send(n + 48);
		send(10);
	}
}
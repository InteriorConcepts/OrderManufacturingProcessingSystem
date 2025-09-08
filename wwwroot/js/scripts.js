window.HostObj =  () => window.chrome.webview.hostObjects.BackendApi;
window.get = async function(methodName, ...params) {
	const res = await HostObj()[methodName](...params);
}
window.sqlFetch = async function(method, fields, filters, limit) {
	const res = await HostObj()[method](fields, filters, limit);
	if (typeof res !== 'string') {
		return;
	}
	const json = JSON.parse(res);
	delete res;
	//console.log(json);
	let dictArr = [];
	for (let i = 0; i < json.length; i++) {
		let temp = {};
		for (let j = 0; j < json[i].length; j++) {
			temp[fields[j]] = json[i][j];
		}
		dictArr.push(temp);
	};
	return dictArr;
}
console.log(await sqlFetch("GetJobOrderLines", ["JobNbr", "ItemNbr", "Description", "Qty"], ["JobNbr='J000035601'"], 1000));
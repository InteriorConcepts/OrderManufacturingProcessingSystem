window.HostObj = () => window.chrome.webview.hostObjects.BackendApi;
window.get = async function (methodName, ...params) {
	const res = await HostObj()[methodName](...params);
};
window.method_fields = {
	"GetRecentOrders": [
		"ColorSetID", "OppOrderID", "OpportunityID", "QuoteID",
		"OrderNumber", "OrderDate", "LineNumber", "SupplyOrderRef",
		"CreationDate", "ChangeDate", "NetProduct"
	]
};
window.sqlFetch = async function (method, limit, ...params) {
	const fields = method_fields[method];
	if (typeof fields == 'undefined') {
		return null;
	}
	const res = await HostObj()[method](limit, ...params);
	if (typeof res !== 'string') {
		return;
	}
	const json = JSON.parse(res);
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
};

console.log(await sqlFetch("GetRecentOrders", "J000035601", 10));
//console.log(await sqlFetch("GetJobOrderLines", ["JobNbr", "ItemNbr", "Description", "Qty"], ["JobNbr='J000035601'"], 1000));
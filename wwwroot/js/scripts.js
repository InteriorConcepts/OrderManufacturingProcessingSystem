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

//console.log(await sqlFetch("GetRecentOrders", 10));
//console.log(await sqlFetch("GetJobOrderLines", ["JobNbr", "ItemNbr", "Description", "Qty"], ["JobNbr='J000035601'"], 1000));

let table = null;
document.addEventListener('DOMContentLoaded', (ev) => {
	
	/** @type {example_queries_GetItemLinesByJobResult[]?} */
	let data = JSON.parse(window.chrome.webview.hostObjects.sync.BackendApi.GetItemLinesByJob_Web("J000035601"));
	table = new DataTable('#dataframe', {
		fixedHeader: {
        headerOffset: 50
    },
		paging: false,
		responsive: true,
		scrollY: true,
		scrollX: true,
		deferRender: true,
		autoWidth: false,
		ordering: true,
		data: data,
		columns: Object.keys(data[0]).filter(k => !k.endsWith("ID")).map(k => ({ data: k, title: k }))
	});
});

class example_queries_GetItemLinesByJobResult {
	/** @type {string} */
	Area;
	/** @type {string} */
	Assembled;
	/** @type {string} */
	AssyNbr;
	/** @type {boolean} */
	BpartnerAvailable;
	/** @type {string} */
	CatalogNbr;
	/** @type {Guid} */
	ChangebyIDOffline;
	/** @type {Date} */
	ChangeDate;
	/** @type {Guid} */
	ChangedbyID;
	/** @type {string} */
	ColorBy;
	/** @type {Guid} */
	ColorSetID;
	/** @type {string} */
	CoreSize;
	/** @type {Guid} */
	CreatedByID;
	/** @type {Date} */
	CreationDate;
	/** @type {boolean} */
	CustomerAvailable;
	/** @type {string} */
	CustOrderNbr;
	/** @type {string} */
	Dept;
	/** @type {number} */
	Depth;
	/** @type {string} */
	Description;
	/** @type {boolean} */
	Explode;
	/** @type {number} */
	FabHeight;
	/** @type {number} */
	Fabwidth;
	/** @type {number} */
	Height;
	/** @type {Guid} */
	IceManufID;
	/** @type {string} */
	IDNbr;
	/** @type {string} */
	ItemCore;
	/** @type {string} */
	ItemFin;
	/** @type {Guid} */
	ItemID;
	/** @type {string} */
	ItemNbr;
	/** @type {string} */
	JobNbr;
	/** @type {number} */
	Multiplier;
	/** @type {string} */
	Option01;
	/** @type {string} */
	Option02;
	/** @type {string} */
	Option03;
	/** @type {string} */
	Option04;
	/** @type {string} */
	Option05;
	/** @type {string} */
	Option06;
	/** @type {string} */
	Option07;
	/** @type {string} */
	Option08;
	/** @type {string} */
	Option09;
	/** @type {string} */
	Option10;
	/** @type {string} */
	PartNbr;
	/** @type {Guid} */
	ProductID;
	/** @type {Guid} */
	ProductLinkID;
	/** @type {number} */
	Qty;
	/** @type {string} */
	QuoteNbr;
	/** @type {number} */
	ScrapFactor;
	/** @type {number} */
	SizeDivisor;
	/** @type {string} */
	SubType;
	/** @type {string} */
	TileIndicator;
	/** @type {string} */
	Type;
	/** @type {string} */
	UofM;
	/** @type {string} */
	Usertag1;
	/** @type {number} */
	Width;
	/** @type {string} */
	WorkCtr;
}

class Guid {
	/** @type {Guid} */
	AllBitsSet;
	/** @type {number} */
	Variant;
	/** @type {number} */
	Version;
}
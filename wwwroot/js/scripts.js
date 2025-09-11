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


function initDataTable(selector, data) {
	$(selector).DataTable({
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
		data: data, // Assuming your JSON has a key for the data array
		columns: Object.keys(data[0]).filter(k => !k.endsWith("ID")).map(c => ({data: c, title: c}))
	});
}

function updateaDataTable(selector, data) {
	const table = $(selector).DataTable();
	table.clear().rows.add(data).draw();
}

async function gzipDecompressBlob(compressedBlob) {

	// Using DecompressionStream (requires browser support for Compression Streams API)
	const decompressedStream = compressedBlob.stream().pipeThrough(new DecompressionStream('gzip'));
	const decompressedResponse = new Response(decompressedStream);
	const json = await decompressedResponse.json();
	return json;
}

async function apiFetch(path, params) {
	if (typeof path !== 'string') {
		return null;
	}
	if (typeof params === 'undefined' || params == null || (typeof params == 'object' && typeof params.length !== 'number')) {
		params = [];
	}
	const res = await fetch(
		`/api/${path}?${(params.join('&'))}`, {
			method: "GET",
			headers: {
				"Accept": "application/json",
				"Accept-Encoding": "gzip, deflate"
			}
		}
	);
	if (res == null) {
		return null;
	}
	if (!res.ok || res.status !== 200) {
		return null;
	}
	
	const blob = await res.blob();
	if (typeof blob === 'undefined' || blob == null) {
		return null;
	}
	alert(blob.type);
	if (blob.type !== 'application/json') {
		return null;
	}

	const json = await gzipDecompressBlob(blob);
	if (!(typeof json === 'object' && typeof json.length === 'number')) {
		return null;
	}

	return json;
}

let table = null;
document.addEventListener('DOMContentLoaded', async (ev) => {
	initDataTable(
		'#dataframe',
		await apiFetch('getItemLinesByJob', ['id=123'])
	);
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
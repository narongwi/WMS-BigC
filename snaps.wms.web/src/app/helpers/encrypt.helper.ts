function enc(str):string {
    return btoa(encodeURIComponent(str).replace(/%([0-9A-F]{2})/g, function(match, p1) {
        return String.fromCharCode(parseInt(p1, 16))
    }))
} 
function dec(str):string {
    if (str!=null) {
        return decodeURIComponent(Array.prototype.map.call(atob(str), function(c) {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2)
        }).join(''))
    }else { return ""; }
}
function gencode() { 
    this.u = Date.now().toString(16) + Math.random().toString(16) + '0'.repeat(16);
    return [this.u.substr(0,8), this.u.substr(8,4), '4000-8' + this.u.substr(13,3), this.u.substr(16,12)].join('-');
}
var basket = new Vue({
    el: "#nav",
    data: {
        list: [{ id: 1, title: "abc" }],
        signin: false,
        basketCount: 0
    },
    mounted: function () {
        var self = this;
        axios.get("/api/account/userinfo", {
            headers: {
                Authorization: "Bearer " + localStorage.getItem("access_token")
            }
        }).then(function (res) {
            self.signin = true;
            self.nickName = res.data.nickName;
        }).catch(function (err) {

        }).then(function () {

        });
        axios.get("/api/basket/count", {
            headers: {
                Authorization: "Bearer " + localStorage.getItem("access_token")
            }
        }).then(function (res) {
            self.basketCount = res.data;
        }).catch(function (err) {

        }).then(function () {

        });
    },
    methods: {

    }
});
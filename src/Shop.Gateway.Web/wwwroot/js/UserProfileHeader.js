var basket = new Vue({
    el: "#nav",
    data: {
        list: [{ id: 1, title: "abc" }],
        signin: false,
        basketCount: 0
    },
    mounted: function () {
        this.LoadUserInfo();
        this.LoadBasket();
    },
    methods: {
        LoadUserInfo: function () {
            var self = this;
            axios.get("http://localhost:9001/api/account/userinfo", {
                headers: {
                    Authorization: "Bearer " + localStorage.getItem("access_token")
                }
            }).then(function (res) {
                self.signin = true;
                self.nickName = res.data.nickName;
            }).catch(function (err) {

            }).then(function () {

            });

        },
        LoadBasket: function () {
            var self = this;
            axios.get("http://localhost:9001/api/basket/count", {
                headers: {
                    Authorization: "Bearer " + localStorage.getItem("access_token")
                }
            }).then(function (res) {
                self.basketCount = res.data;
            }).catch(function (err) {

            }).then(function () {

            });
        }
    }
});
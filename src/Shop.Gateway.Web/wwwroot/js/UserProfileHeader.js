new Vue({
    el: "#UserProfileHeader",
    data: {
        list: [{ id: 1, title: "abc" }],
        signin: false
    },
    mounted: function () {
        var self = this;
        axios.get("/api/account/userinfo", {
            headers: {
                Authorization: "Bearer " + localStorage.getItem("access_token")
            }
        })
            .then(function (res) {
                self.signin = true;
                self.nickName = res.data.nickName;
            })
            .catch(function (err) {

            })
            .then(function () {

            });
    },
    methods: {

    }
});
db.createUser(
    {
        user: "root",
        pwd: "universe",
        roles: [
            {
                role: "readWrite",
                db: "iotworld"
            }
        ]
    }
);
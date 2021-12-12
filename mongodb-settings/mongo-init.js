db.createUser(
    {
        user: "prometheus",
        pwd: "engineer",
        roles: [
            {
                role: "readWrite",
                db: "iotworld"
            }
        ]
    }
);
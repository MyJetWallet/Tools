# kube-restart-pods

```
    - name: Restart service
      uses: MyJetWallet/Tools@restart-pod
      env:
        KUBE_CONFIG_DATA: ${{ secrets.KUBE_CONFIG_DATA }}
        NAMESPACE: tools
        POD: blockchain-node-statuses
```
